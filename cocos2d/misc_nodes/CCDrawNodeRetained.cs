using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cocos2D
{
    /// <summary>
    /// Identifies a shape drawn in a CCDrawNodeRetained, allowing
    /// individual shapes to be moved, recolored, or removed without
    /// rebuilding the entire vertex buffer.
    /// </summary>
    public class CCShapeHandle
    {
        internal int StartIndex;
        internal int VertexCount;
        internal CCDrawNodeRetained Owner;
        internal bool Active = true;

        /// <summary>
        /// Whether this shape is still active (not removed).
        /// </summary>
        public bool IsActive { get { return Active; } }
    }

    /// <summary>
    /// A retained-mode draw node where individual shapes can be added, moved,
    /// recolored, or removed without clearing and rebuilding the entire buffer.
    ///
    /// Use this instead of CCDrawNode when you have many shapes and only a few
    /// change per frame. The immediate-mode Clear() + redraw pattern still works
    /// if needed.
    ///
    /// <code>
    /// var node = new CCDrawNodeRetained();
    /// var dot = node.AddDot(center, 10f, CCColor4F.Red);
    /// // Later, move or recolor without full rebuild:
    /// node.MoveShape(dot, newCenter);
    /// node.RecolorShape(dot, CCColor4F.Blue);
    /// node.RemoveShape(dot);
    /// </code>
    /// </summary>
    public class CCDrawNodeRetained : CCNode
    {
        const int DefaultBufferSize = 512;

        private List<VertexPositionColor> m_vertices;
        private List<CCShapeHandle> m_shapes;
        private CCBlendFunc m_blendFunc;
        private VertexPositionColor[] m_drawBuffer;
        private bool m_dirty;

        public CCDrawNodeRetained()
        {
            m_blendFunc = CCBlendFunc.AlphaBlend;
            m_vertices = new List<VertexPositionColor>(DefaultBufferSize);
            m_shapes = new List<CCShapeHandle>();
            m_dirty = true;
        }

        public CCBlendFunc BlendFunc
        {
            get { return m_blendFunc; }
            set { m_blendFunc = value; }
        }

        /// <summary>
        /// Number of active shapes in this node.
        /// </summary>
        public int ShapeCount
        {
            get { return m_shapes.Count; }
        }

        /// <summary>
        /// Total vertex count across all shapes.
        /// </summary>
        public int VertexCount
        {
            get { return m_vertices.Count; }
        }

        #region Add Shapes

        /// <summary>
        /// Adds a filled dot and returns a handle for later manipulation.
        /// </summary>
        public CCShapeHandle AddDot(CCPoint pos, float radius, CCColor4F color)
        {
            if (radius >= 4f)
            {
                return AddFilledCircle(pos, radius, color);
            }

            int start = m_vertices.Count;
            var cl = new Color(color.R, color.G, color.B, color.A);

            var a = new VertexPositionColor(new Vector3(pos.X - radius, pos.Y - radius, 0), cl);
            var b = new VertexPositionColor(new Vector3(pos.X - radius, pos.Y + radius, 0), cl);
            var c = new VertexPositionColor(new Vector3(pos.X + radius, pos.Y + radius, 0), cl);
            var d = new VertexPositionColor(new Vector3(pos.X + radius, pos.Y - radius, 0), cl);

            m_vertices.Add(a);
            m_vertices.Add(b);
            m_vertices.Add(c);
            m_vertices.Add(a);
            m_vertices.Add(c);
            m_vertices.Add(d);

            return CreateHandle(start, 6);
        }

        /// <summary>
        /// Adds a filled circle and returns a handle.
        /// </summary>
        public CCShapeHandle AddFilledCircle(CCPoint center, float radius, CCColor4F color, int segments = 32)
        {
            if (segments < 3) segments = 3;

            int start = m_vertices.Count;
            var cl = new Color(color.R, color.G, color.B, color.A);
            var centerVertex = new VertexPositionColor(new Vector3(center.X, center.Y, 0), cl);
            float increment = MathHelper.TwoPi / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * increment;
                float angle2 = (i + 1) * increment;

                var v1 = new VertexPositionColor(
                    new Vector3(center.X + (float)Math.Cos(angle1) * radius,
                                center.Y + (float)Math.Sin(angle1) * radius, 0), cl);
                var v2 = new VertexPositionColor(
                    new Vector3(center.X + (float)Math.Cos(angle2) * radius,
                                center.Y + (float)Math.Sin(angle2) * radius, 0), cl);

                m_vertices.Add(centerVertex);
                m_vertices.Add(v1);
                m_vertices.Add(v2);
            }

            return CreateHandle(start, segments * 3);
        }

        /// <summary>
        /// Adds a filled triangle and returns a handle.
        /// </summary>
        public CCShapeHandle AddTriangle(CCPoint a, CCPoint b, CCPoint c, CCColor4F color)
        {
            int start = m_vertices.Count;
            var cl = new Color(color.R, color.G, color.B, color.A);

            m_vertices.Add(new VertexPositionColor(new Vector3(a.X, a.Y, 0), cl));
            m_vertices.Add(new VertexPositionColor(new Vector3(b.X, b.Y, 0), cl));
            m_vertices.Add(new VertexPositionColor(new Vector3(c.X, c.Y, 0), cl));

            return CreateHandle(start, 3);
        }

        /// <summary>
        /// Adds a filled rectangle and returns a handle.
        /// </summary>
        public CCShapeHandle AddRect(CCRect rect, CCColor4F color)
        {
            int start = m_vertices.Count;
            var cl = new Color(color.R, color.G, color.B, color.A);

            float x1 = rect.MinX, y1 = rect.MinY;
            float x2 = rect.MaxX, y2 = rect.MaxY;

            var bl = new VertexPositionColor(new Vector3(x1, y1, 0), cl);
            var br = new VertexPositionColor(new Vector3(x2, y1, 0), cl);
            var tr = new VertexPositionColor(new Vector3(x2, y2, 0), cl);
            var tl = new VertexPositionColor(new Vector3(x1, y2, 0), cl);

            m_vertices.Add(bl);
            m_vertices.Add(br);
            m_vertices.Add(tr);
            m_vertices.Add(bl);
            m_vertices.Add(tr);
            m_vertices.Add(tl);

            return CreateHandle(start, 6);
        }

        /// <summary>
        /// Adds a line segment and returns a handle.
        /// </summary>
        public CCShapeHandle AddSegment(CCPoint from, CCPoint to, float radius, CCColor4F color)
        {
            int start = m_vertices.Count;
            var cl = new Color(color.R, color.G, color.B, color.A);

            var a = from;
            var b = to;
            var n = CCPoint.Normalize(CCPoint.Perp(a - b));
            var t = CCPoint.Perp(n);
            var nw = n * radius;
            var tw = t * radius;

            var v0 = b - (nw + tw);
            var v1 = b + (nw - tw);
            var v2 = b - nw;
            var v3 = b + nw;
            var v4 = a - nw;
            var v5 = a + nw;
            var v6 = a - (nw - tw);
            var v7 = a + (nw + tw);

            m_vertices.Add(new VertexPositionColor(v0, cl));
            m_vertices.Add(new VertexPositionColor(v1, cl));
            m_vertices.Add(new VertexPositionColor(v2, cl));
            m_vertices.Add(new VertexPositionColor(v3, cl));
            m_vertices.Add(new VertexPositionColor(v1, cl));
            m_vertices.Add(new VertexPositionColor(v2, cl));
            m_vertices.Add(new VertexPositionColor(v3, cl));
            m_vertices.Add(new VertexPositionColor(v4, cl));
            m_vertices.Add(new VertexPositionColor(v2, cl));
            m_vertices.Add(new VertexPositionColor(v3, cl));
            m_vertices.Add(new VertexPositionColor(v4, cl));
            m_vertices.Add(new VertexPositionColor(v5, cl));
            m_vertices.Add(new VertexPositionColor(v6, cl));
            m_vertices.Add(new VertexPositionColor(v4, cl));
            m_vertices.Add(new VertexPositionColor(v5, cl));
            m_vertices.Add(new VertexPositionColor(v6, cl));
            m_vertices.Add(new VertexPositionColor(v7, cl));
            m_vertices.Add(new VertexPositionColor(v5, cl));

            return CreateHandle(start, 18);
        }

        #endregion

        #region Manipulate Shapes

        /// <summary>
        /// Moves all vertices of a shape by the given offset.
        /// </summary>
        public void MoveShape(CCShapeHandle handle, CCPoint offset)
        {
            ValidateHandle(handle);
            var off = new Vector3(offset.X, offset.Y, 0);

            for (int i = handle.StartIndex; i < handle.StartIndex + handle.VertexCount; i++)
            {
                var v = m_vertices[i];
                v.Position += off;
                m_vertices[i] = v;
            }

            m_dirty = true;
        }

        /// <summary>
        /// Sets the position of all vertices by translating the shape so its
        /// centroid is at the given position.
        /// </summary>
        public void SetShapePosition(CCShapeHandle handle, CCPoint position)
        {
            ValidateHandle(handle);

            // Compute current centroid
            float cx = 0, cy = 0;
            for (int i = handle.StartIndex; i < handle.StartIndex + handle.VertexCount; i++)
            {
                cx += m_vertices[i].Position.X;
                cy += m_vertices[i].Position.Y;
            }
            cx /= handle.VertexCount;
            cy /= handle.VertexCount;

            var off = new Vector3(position.X - cx, position.Y - cy, 0);
            for (int i = handle.StartIndex; i < handle.StartIndex + handle.VertexCount; i++)
            {
                var v = m_vertices[i];
                v.Position += off;
                m_vertices[i] = v;
            }

            m_dirty = true;
        }

        /// <summary>
        /// Changes the color of all vertices in a shape.
        /// </summary>
        public void RecolorShape(CCShapeHandle handle, CCColor4F color)
        {
            ValidateHandle(handle);
            var cl = new Color(color.R, color.G, color.B, color.A);

            for (int i = handle.StartIndex; i < handle.StartIndex + handle.VertexCount; i++)
            {
                var v = m_vertices[i];
                v.Color = cl;
                m_vertices[i] = v;
            }

            m_dirty = true;
        }

        /// <summary>
        /// Sets the opacity of all vertices in a shape.
        /// </summary>
        public void SetShapeOpacity(CCShapeHandle handle, byte opacity)
        {
            ValidateHandle(handle);

            for (int i = handle.StartIndex; i < handle.StartIndex + handle.VertexCount; i++)
            {
                var v = m_vertices[i];
                v.Color = new Color(v.Color.R, v.Color.G, v.Color.B, opacity);
                m_vertices[i] = v;
            }

            m_dirty = true;
        }

        /// <summary>
        /// Removes a shape from the draw node. This compacts the vertex buffer
        /// and invalidates all handles that were added after this shape.
        /// For best performance, remove shapes in reverse order of creation
        /// or use Clear() when removing many shapes.
        /// </summary>
        public void RemoveShape(CCShapeHandle handle)
        {
            ValidateHandle(handle);

            int start = handle.StartIndex;
            int count = handle.VertexCount;

            m_vertices.RemoveRange(start, count);

            handle.Active = false;
            m_shapes.Remove(handle);

            // Update start indices for all shapes after the removed one
            for (int i = 0; i < m_shapes.Count; i++)
            {
                if (m_shapes[i].StartIndex > start)
                {
                    m_shapes[i].StartIndex -= count;
                }
            }

            m_dirty = true;
        }

        #endregion

        /// <summary>
        /// Clears all shapes and vertices.
        /// </summary>
        public void Clear()
        {
            m_vertices.Clear();
            for (int i = 0; i < m_shapes.Count; i++)
            {
                m_shapes[i].Active = false;
            }
            m_shapes.Clear();
            m_drawBuffer = null;
            m_dirty = true;
        }

        public override void Draw()
        {
            if (m_vertices.Count == 0) return;

            if (m_dirty)
            {
                m_dirty = false;
                m_drawBuffer = m_vertices.ToArray();
            }

            if (m_drawBuffer != null && m_drawBuffer.Length >= 3)
            {
                CCDrawManager.TextureEnabled = false;
                CCDrawManager.BlendFunc(m_blendFunc);
                CCDrawManager.DrawPrimitives(PrimitiveType.TriangleList, m_drawBuffer, 0, m_drawBuffer.Length / 3);
            }
        }

        private CCShapeHandle CreateHandle(int startIndex, int vertexCount)
        {
            var handle = new CCShapeHandle
            {
                StartIndex = startIndex,
                VertexCount = vertexCount,
                Owner = this,
                Active = true
            };
            m_shapes.Add(handle);
            m_dirty = true;
            return handle;
        }

        private void ValidateHandle(CCShapeHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException("handle");
            if (!handle.Active)
                throw new InvalidOperationException("Shape handle has been removed.");
            if (handle.Owner != this)
                throw new InvalidOperationException("Shape handle belongs to a different CCDrawNodeRetained.");
        }
    }
}
