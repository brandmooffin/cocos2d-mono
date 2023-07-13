/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2011-2012 openxlive.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using System;
using Microsoft.Xna.Framework;

namespace Cocos2D
{
    public struct CCAffineTransform
    {
        public static readonly CCAffineTransform Identity = new CCAffineTransform(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);

        public float a, b, c, d;
        public float tx, ty;

        Matrix xnaMatrix;

        #region Properties

        internal Matrix XnaMatrix { get { return xnaMatrix; } }

        public float A
        {
            get { return xnaMatrix.M11; }
            set { xnaMatrix.M11 = value; }
        }

        public float B
        {
            get { return xnaMatrix.M12; }
            set { xnaMatrix.M12 = value; }
        }

        public float C
        {
            get { return xnaMatrix.M21; }
            set { xnaMatrix.M21 = value; }
        }

        public float D
        {
            get { return xnaMatrix.M22; }
            set { xnaMatrix.M22 = value; }
        }

        public float Tx
        {
            get { return xnaMatrix.M41; }
            set { xnaMatrix.M41 = value; }
        }

        public float Ty
        {
            get { return xnaMatrix.M42; }
            set { xnaMatrix.M42 = value; }
        }

        public float Tz
        {
            get { return xnaMatrix.M43; }
            set { xnaMatrix.M43 = value; }
        }

        public float Scale
        {
            set { ScaleX = value; ScaleY = value; }
        }

        public float ScaleX
        {
            get
            {
                float a2 = (float)Math.Pow(A, 2);
                float b2 = (float)Math.Pow(B, 2);
                return (float)Math.Sqrt(a2 + b2);
            }

            set
            {
                float rotX = RotationY;
                A = value * (float)Math.Cos(rotX);
                B = value * (float)Math.Sin(rotX);
            }
        }

        public float ScaleY
        {
            get
            {
                float d2 = (float)Math.Pow(D, 2);
                float c2 = (float)Math.Pow(C, 2);
                return (float)Math.Sqrt(d2 + c2);
            }

            set
            {
                double rotY = RotationY;

                C = -value * (float)Math.Sin(rotY);
                D = value * (float)Math.Cos(rotY);
            }
        }

        public float Rotation
        {
            set
            {
                float rotX = RotationX;
                float rotY = RotationY;

                float scaleX = ScaleX;
                float scaleY = ScaleY;

                A = scaleX * (float)Math.Cos(value);
                B = scaleX * (float)Math.Sin(value);
                C = -scaleY * (float)Math.Sin(rotY - rotX + value);
                D = scaleY * (float)Math.Cos(rotY - rotX + value);
            }
        }

        public float RotationX
        {
            get { return (float)Math.Atan2(B, A); }
        }

        public float RotationY
        {
            get { return (float)Math.Atan2(-C, D); }
        }

        public CCAffineTransform Inverse
        {
            get { return new CCAffineTransform(Matrix.Invert(XnaMatrix)); }
        }

        //public CCAffineTransform Inverse
        //{
        //    get { return new CCAffineTransform(Matrix.Invert(XnaMatrix)); }
        //}

        #endregion Properties

        public CCAffineTransform(float a, float b, float c, float d, float tx, float ty)
        {
            xnaMatrix = Matrix.Identity;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.tx = tx;
            this.ty = ty;
        }

        internal CCAffineTransform(Matrix xnaMatrixIn) : this()
        {
            xnaMatrix = xnaMatrixIn;
        }

        public static CCPoint Transform(CCPoint point, CCAffineTransform t)
        {
            return new CCPoint(
                t.a * point.X + t.c * point.Y + t.tx,
                t.b * point.X + t.d * point.Y + t.ty
                );
        }

        public static CCSize Transform(CCSize size, CCAffineTransform t)
        {
            var s = new CCSize();
            s.Width = (float) ((double) t.a * size.Width + (double) t.c * size.Height);
            s.Height = (float) ((double) t.b * size.Width + (double) t.d * size.Height);
            return s;
        }

        public void Transform(ref float x, ref float y, ref float z)
        {
            Vector3 vector = new Vector3(x, y, z);
            Vector3.Transform(ref vector, ref xnaMatrix, out vector);
            x = vector.X;
            y = vector.Y;
            z = vector.Z;
        }

        public void Transform(ref CCV3F_C4B_T2F quadPoint)
        {
            var x = quadPoint.Vertices.X;
            var y = quadPoint.Vertices.Y;
            var z = quadPoint.Vertices.Z;
            Transform(ref x, ref y, ref z);
            quadPoint.Vertices.X = x;
            quadPoint.Vertices.Y = y;
            quadPoint.Vertices.Z = z;
        }

        //public void Transform(ref CCV3F_C4B quadPoint)
        //{
        //    var x = quadPoint.Vertices.X;
        //    var y = quadPoint.Vertices.Y;
        //    var z = quadPoint.Vertices.Z;
        //    Transform(ref x, ref y, ref z);
        //    quadPoint.Vertices.X = x;
        //    quadPoint.Vertices.Y = y;
        //    quadPoint.Vertices.Z = z;
        //}

        public void Transform(ref CCV3F_C4B_T2F_Quad quad)
        {
            Transform(ref quad.TopLeft);
            Transform(ref quad.TopRight);
            Transform(ref quad.BottomLeft);
            Transform(ref quad.BottomRight);
        }

        public CCV3F_C4B_T2F_Quad Transform(CCV3F_C4B_T2F_Quad quad)
        {
            Transform(ref quad);

            return quad;
        }

        public static CCAffineTransform Translate(CCAffineTransform t, float tx, float ty)
        {
            return new CCAffineTransform(t.a, t.b, t.c, t.d, t.tx + t.a * tx + t.c * ty, t.ty + t.b * tx + t.d * ty);
        }

        public static CCAffineTransform Rotate(CCAffineTransform t, float anAngle)
        {
            var fSin = (float) Math.Sin(anAngle);
            var fCos = (float) Math.Cos(anAngle);

            return new CCAffineTransform(t.a * fCos + t.c * fSin,
                                         t.b * fCos + t.d * fSin,
                                         t.c * fCos - t.a * fSin,
                                         t.d * fCos - t.b * fSin,
                                         t.tx,
                                         t.ty);
        }

        //public static CCAffineTransform Scale(CCAffineTransform t, float sx, float sy)
        //{
        //    return new CCAffineTransform(t.a * sx, t.b * sx, t.c * sy, t.d * sy, t.tx, t.ty);
        //}

        /// <summary>
        /// Concatenate `t2' to `t1' and return the result:
        /// t' = t1 * t2 */s
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static CCAffineTransform Concat(CCAffineTransform t1, CCAffineTransform t2)
        {
            return new CCAffineTransform(t1.a * t2.a + t1.b * t2.c, t1.a * t2.b + t1.b * t2.d, //a,b
                                         t1.c * t2.a + t1.d * t2.c, t1.c * t2.b + t1.d * t2.d, //c,d
                                         t1.tx * t2.a + t1.ty * t2.c + t2.tx, //tx
                                         t1.tx * t2.b + t1.ty * t2.d + t2.ty); //ty
        }

        public static void Concat(ref CCAffineTransform t1, ref CCAffineTransform t2, out CCAffineTransform tOut)
        {
            Matrix concatMatrix;
            Matrix.Multiply(ref t1.xnaMatrix, ref t2.xnaMatrix, out concatMatrix);
            tOut = new CCAffineTransform(concatMatrix);
        }

        ///// <summary>
        ///// Get/set the XNA matrix of this transform. This matrix will assume z=0.
        ///// </summary>
        //public Matrix XnaMatrix
        //{
        //    get
        //    {
        //        Matrix m = Matrix.Identity;
        //        m.M11 = a;
        //        m.M21 = c;
        //        m.M12 = b;
        //        m.M22 = d;
        //        m.M41 = tx;
        //        m.M42 = ty;
        //        m.M43 = 0; // Always at Z=0
        //        return (m);
        //    }
        //    set
        //    {
        //        a = value.M11;
        //        c = value.M21;
        //        b = value.M12;
        //        d = value.M22;
        //        tx = value.M41;
        //        ty = value.M42;
        //        // Ignore z
        //    }
        //}

        public void Concat(CCAffineTransform m)
        {
            Concat(ref m);
        }

        public void Concat(ref CCAffineTransform m)
        {
            float t_a = a;
            float t_b = b;
            float t_c = c;
            float t_d = d;
            float t_tx = tx;
            float t_ty = ty;

            float m_a = m.a;
            float m_b = m.b;
            float m_c = m.c;
            float m_d = m.d;

            a = m_a * t_a + m_c * t_b;
            b = m_b * t_a + m_d * t_b;
            c = m_a * t_c + m_c * t_d;
            d = m_b * t_c + m_d * t_d;

            tx = m_a * t_tx + m_c * t_ty + m.tx;
            ty = m_b * t_tx + m_d * t_ty + m.ty;
        }

        /// <summary>
        ///  Return true if `t1' and `t2' are equal, false otherwise. 
        /// </summary>
        public static bool Equal(CCAffineTransform t1, CCAffineTransform t2)
        {
            return (t1.a == t2.a && t1.b == t2.b && t1.c == t2.c && t1.d == t2.d && t1.tx == t2.tx && t1.ty == t2.ty);
        }

        public static CCAffineTransform Invert(CCAffineTransform t)
        {
            float determinant = 1 / (t.a * t.d - t.b * t.c);

            return new CCAffineTransform(determinant * t.d, -determinant * t.b, -determinant * t.c, determinant * t.a,
                                         determinant * (t.c * t.ty - t.d * t.tx), determinant * (t.b * t.tx - t.a * t.ty));
        }

        public float TranslationX
        {
            get { return tx; }
            set { tx = value; }
        }

        public float TranslationY
        {
            get { return ty; }
            set { ty = value; }
        }

        public void SetTranslation(float x, float y)
        {
            tx = x;
            ty = y;
        }

        public void ConcatTranslation(float x, float y)
        {
            tx += x;
            ty += y;
        }

        public void SetScale(float scaleX, float scaleY)
        {
            SetScaleRotation(scaleX, scaleY, GetRotation());
        }

        public float GetScaleX()
        {
            float a2 = (float)Math.Pow(a, 2);
            float b2 = (float)Math.Pow(b, 2);
            return (float)Math.Sqrt(a2 + b2);
        }

        public void SetScaleX(float scaleX)
        {
            float rotX = GetRotationX();
            a = scaleX * (float)Math.Cos(rotX);
            b = scaleX * (float)Math.Sin(rotX);
        }

        public float GetScaleY()
        {
            float d2 = (float)Math.Pow(d, 2);
            float c2 = (float)Math.Pow(c, 2);
            return (float)Math.Sqrt(d2 + c2);
        }

        public void SetScaleY(float scaleY)
        {
            double rotY = GetRotationY();

            c = -scaleY * (float)Math.Sin(rotY);
            d = scaleY * (float)Math.Cos(rotY);
        }

        public void ConcatScale(float xscale, float yscale)
        {
            a *= xscale;
            c *= yscale;
            b *= xscale;
            d *= yscale;
        }

        public float GetRotation()
        {
            return GetRotationX();
        }

        public float GetRotationX()
        {
            return (float)Math.Atan2(b, a);
        }

        public float GetRotationY()
        {
            return (float)Math.Atan2(-c, d);
        }

        /// Set rotation in radians, scales component are unchanged.
        public void SetRotation(float rotation)
        {
            float rotX = GetRotationX();
            float rotY = GetRotationY();

            float scaleX = GetScaleX();
            float scaleY = GetScaleY();

            a = scaleX * (float)Math.Cos(rotation);
            b = scaleX * (float)Math.Sin(rotation);
            c = -scaleY * (float)Math.Sin(rotY - rotX + rotation);
            d = scaleY * (float)Math.Cos(rotY - rotX + rotation);
        }

        /// Set the scale & rotation part of the CCAffineTransform. angle in radians.
        public void SetScaleRotation(float scaleX, float scaleY, float angle)
        {
            var cosa = (float)Math.Cos(angle);
            var sina = (float)Math.Sin(angle);

            a = scaleX * cosa;
            c = scaleY * -sina;
            b = scaleX * sina;
            d = scaleY * cosa;
        }

        public void SetLerp(CCAffineTransform m1, CCAffineTransform m2, float t)
        {
            a = MathHelper.Lerp(m1.a, m2.a, t);
            b = MathHelper.Lerp(m1.b, m2.b, t);
            c = MathHelper.Lerp(m1.c, m2.c, t);
            d = MathHelper.Lerp(m1.d, m2.d, t);
            tx = MathHelper.Lerp(m1.tx, m2.tx, t);
            ty = MathHelper.Lerp(m1.ty, m2.ty, t);
        }

        //public static void Lerp(ref CCAffineTransform m1, ref CCAffineTransform m2, float t, out CCAffineTransform res)
        //{
        //    res.a = MathHelper.Lerp(m1.a, m2.a, t);
        //    res.b = MathHelper.Lerp(m1.b, m2.b, t);
        //    res.c = MathHelper.Lerp(m1.c, m2.c, t);
        //    res.d = MathHelper.Lerp(m1.d, m2.d, t);
        //    res.tx = MathHelper.Lerp(m1.tx, m2.tx, t);
        //    res.ty = MathHelper.Lerp(m1.ty, m2.ty, t);
        //}
        public void Lerp(CCAffineTransform m1, CCAffineTransform m2, float t)
        {
            Matrix.Lerp(ref m1.xnaMatrix, ref m2.xnaMatrix, t, out xnaMatrix);
        }

        public void Transform(ref float x, ref float y)
        {
            var tmpX = a * x + c * y + tx;
            y = b * x + d * y + ty;
            x = tmpX;
        }

        public void Transform(ref int x, ref int y)
        {
            var tmpX = a * x + c * y + tx;
            y = (int)(b * x + d * y + ty);
            x = (int)tmpX;
        }

        public void Transform(float x, float y, out float xresult, out float yresult)
        {
            xresult = a * x + c * y + tx;
            yresult = b * x + d * y + ty;
        }

        public CCPoint Transform(CCPoint point)
        {
            return new CCPoint(
                a * point.X + c * point.Y + tx,
                b * point.X + d * point.Y + ty
                );
        }

        public void Transform(ref CCPoint point)
        {
            var tmpX = a * point.X + c * point.Y + tx;
            point.Y = b * point.X + d * point.Y + ty;
            point.X = tmpX;
        }

        public CCRect Transform(CCRect rect)
        {
            float top = rect.MinY;
            float left = rect.MinX;
            float right = rect.MaxX;
            float bottom = rect.MaxY;

            CCPoint topLeft = new CCPoint(left, top);  
            CCPoint topRight = new CCPoint(right, top);
            CCPoint bottomLeft = new CCPoint(left, bottom);
            CCPoint bottomRight = new CCPoint(right, bottom);

            Transform(ref topLeft);
            Transform(ref topRight);
            Transform(ref bottomLeft);
            Transform(ref bottomRight);

            float minX = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
            float maxX = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X));
            float minY = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));
            float maxY = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y));

            return new CCRect(minX, minY, (maxX - minX), (maxY - minY));
        }

        public void Transform(ref CCRect rect)
        {
            float top = rect.MinY;
            float left = rect.MinX;
            float right = rect.MaxX;
            float bottom = rect.MaxY;

            CCPoint topLeft = new CCPoint(left, top);
            CCPoint topRight = new CCPoint(right, top);
            CCPoint bottomLeft = new CCPoint(left, bottom);
            CCPoint bottomRight = new CCPoint(right, bottom);

            Transform(ref topLeft);
            Transform(ref topRight);
            Transform(ref bottomLeft);
            Transform(ref bottomRight);

            float minX = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
            float maxX = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X));
            float minY = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));
            float maxY = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y));

            rect.Origin.X = minX;
            rect.Origin.Y = minY;
            rect.Size.Width = maxX - minX;
            rect.Size.Height = maxY - minY;
        }

        public static CCRect Transform(CCRect rect, CCAffineTransform anAffineTransform)
        {
            return anAffineTransform.Transform(rect);
        }

        public bool Equals(ref CCAffineTransform t)
        {
            return a == t.a && b == t.b && c == t.c && d == t.d && tx == t.tx && ty == t.ty;
        }

        #region Operators

        public static CCAffineTransform operator +(CCAffineTransform affineTransform1, CCAffineTransform affineTransform2)
        {
            return new CCAffineTransform(affineTransform1.xnaMatrix + affineTransform2.xnaMatrix);
        }

        public static CCAffineTransform operator /(CCAffineTransform affineTransform1, CCAffineTransform affineTransform2)
        {
            return new CCAffineTransform(affineTransform1.xnaMatrix / affineTransform2.xnaMatrix);
        }

        public static CCAffineTransform operator /(CCAffineTransform affineTransform, float divider)
        {
            return new CCAffineTransform(affineTransform.xnaMatrix / divider);
        }

        public static bool operator ==(CCAffineTransform affineTransform1, CCAffineTransform affineTransform2)
        {
            return affineTransform1.xnaMatrix == affineTransform2.xnaMatrix;
        }

        public static bool operator !=(CCAffineTransform affineTransform1, CCAffineTransform affineTransform2)
        {
            return affineTransform1.xnaMatrix != affineTransform2.xnaMatrix;
        }

        public static CCAffineTransform operator -(CCAffineTransform affineTransform1, CCAffineTransform affineTransform2)
        {
            return new CCAffineTransform(affineTransform1.xnaMatrix - affineTransform2.xnaMatrix);
        }

        public static CCAffineTransform operator *(CCAffineTransform affinematrix1, CCAffineTransform affinematrix2)
        {
            return new CCAffineTransform(affinematrix1.xnaMatrix * affinematrix2.xnaMatrix);
        }

        public static CCAffineTransform operator -(CCAffineTransform affineTransform1)
        {
            Matrix transformedMat = -(affineTransform1.xnaMatrix);
            return new CCAffineTransform(transformedMat);
        }

        #endregion Operators
    }
}