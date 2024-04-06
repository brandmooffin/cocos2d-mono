using Cocos2D;

namespace cocos2d_mono.Tests.LabelTest
{
    public class SystemLabelTest : AtlasDemo
    {
        CCLabel smallSystemLabel;
        CCLabel mediumSystemLabel;
        CCLabel largeSystemLabel;
        CCLabel smallAdjustDimensionSystemLabel;
        CCLabel multiLineSystemLabel;
        CCLabel japaneseSystemLabel;

        string[] fontList = new string[] {
            "따뜻한 있으며, 있는 인간에 보는 품으며. 그들의 가는 사는가 이상이 인생을 풀밭에 황금시대를 때문이다. 풀이 든 끝에 때에. 이것은 눈이 피고 공자는 칼이다, 얼마나 하는 뭇 있는 이 바이며. 튼튼하며. 얼마나 꽃이 우리의 이것이다.\r\n",
            "그들은 찬미를 위하여서. 속잎나고. 예가 인생에 가는 그리하였는가? 열락의 물방아 풀이 공자는 약동하다. 희망의 위하여, 그러므로 풀밭에 얼음에 아니다. 그들의 주며, 얼음 봄바람을 목숨을 얼마나 충분히 수 쓸쓸하랴? 구할 피가 이상이 것은 인생을 능히 우리의 열락의 것이다. 이상은 원질이 만물은 실로 피에 모래뿐일 커다란 약동하다.\r\n",
            "食う寝る処に住む処、寿限無、寿限無。シューリンガンのグーリンダイ。長久命の長助。やぶら小路の藪柑子。五劫の擦り切れ。水行末 雲来末 風来末、海砂利水魚の。水行末 雲来末 風来末、五劫の擦り切れ。やぶら小路の藪柑子、食う寝る処に住む処、海砂利水魚の。長久命の長助。\r\n",
            "長久命の長助。パイポパイポ パイポのシューリンガン、五劫の擦り切れ、水行末 雲来末 風来末、海砂利水魚の。シューリンガンのグーリンダイ、寿限無、寿限無、五劫の擦り切れ。\r\n",
            "食う寝る処に住む処。グーリンダイのポンポコピーのポンポコナーの。やぶら小路の藪柑子、長久命の長助、海砂利水魚の。海砂利水魚の。水行末 雲来末 風来末。寿限無、寿限無。五劫の擦り切れ、パイポパイポ パイポのシューリンガン、水行末 雲来末 風来末。パイポパイポ パイポのシューリンガン、やぶら小路の藪柑子、シューリンガンのグーリンダイ。\r\n",
            "見ぎ民内ぞ日美むべず雑果や韓減ア記直でては質約場わス真惑全ムトメ発突林ツクミワ都4天ヱハタ黒記トマキ出76作百ざえ。楽ク育整クすこほ申山テロウタ文行むほ対山公ツソウ断地長ね省文らごみや半最強はうり況級だれうり視告クサウシ募彦ゆ更残ウアカヘ幕71彫慰腐ぼ。稿ラゆ代北杯ょ納投シサチス店維ホヲレチ姿部チメ大供ろぱそン困命91分ヌエ補域ゅンっ攻著みラへ景同ラシサ越療評領続イんれご。\r\n",
            "ち毛譜へょゅりまへしす。とせりろ知知課以区差魔雲氏離露素くくせ絵巣てなあき都ゃっひゃさはか列雲舳めっる列素夜御津イツレオエへう阿氏野列以ね雲瀬他雲ねろっひふ、や雲毛魔尾た津れそやも、夜絵。\r\n",
            "ゃぬもい。おけ保以名尾。めろふみお日他あよ。のシコチムモ毛以樹っり離露遊留鵜ききもよん遊野にエタハニセーレ離夜擢魔派。ゅてれ夜他かか以二んっな「る遊巣絵列」は樹以まく御毛手。都夜離舳素模むすゆかとり無津夜阿擢野りう模ろしもく根絵みはぬも尾魔日都二やぬそ区ねもころ無屋派列譜りと魔二夜めもっらょむ巣雲。",
            "Lorem ipsum dolor sit amet, magna maiestatis an cum, sit ne dicunt inciderint, te ius ridens tritani. Cu veri justo his. In eum novum vivendo. Dicant commodo sed te, mucius maiestatis duo ad. Ad vis mutat clita, vel ei urbanitas concludaturque. Has falli feugiat ei, diceret mandamus te eum.\r\n"
        };

        CCScene scene = new CCScene();

        public SystemLabelTest()
        {
            CCSize s = CCDirector.SharedDirector.WinSize;


            smallSystemLabel = new CCLabel("Test with small size", "Arial", 12)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y - 200)
            };


            mediumSystemLabel = new CCLabel("Test with medium size", "Arial", 32)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y - 100)
            };

            largeSystemLabel = new CCLabel("Test with large size", "Arial", 96)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y)
            };


            smallAdjustDimensionSystemLabel = new CCLabel("Test with adjusted dimensions", "Arial", 12)
            {
                Position = new CCPoint(s.Center.X - 300, s.Center.Y + 250)
            };

            multiLineSystemLabel = new CCLabel("Test with multi\nlines and different\ncharacters\n複数行と異なる文字でテストする\nマルチでテストする\n行と異なる", "Arial", 12)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y + 125)
            };

            japaneseSystemLabel = new CCLabel("矢印の色がセルの背景色と同じの場合、それがたどる方向です。", "Arial", 28) 
            {
                Position = new CCPoint(s.Center.X + 150, s.Center.Y + 150),
                Dimensions = new CCSize(150, 0),
                LineBreakMode = CCTextLineBreakMode.SmartBreak
            };

            japaneseSystemLabel.SystemFontSpacing = 10;

            //Schedule(UpdateLabel, 2);

            AddChild(smallSystemLabel);
            AddChild(smallAdjustDimensionSystemLabel);
            AddChild(multiLineSystemLabel);
            AddChild(mediumSystemLabel);
            AddChild(largeSystemLabel);
            AddChild(japaneseSystemLabel);
            //scene.AddChild(japaneseSystemLabel);

            smallAdjustDimensionSystemLabel.Dimensions = new CCSize(500, 0);
            string text = "Thank you for visiting the cocos2d-mono tests\nPlease help us by donating to our project\nYou can find us at cocos2d-mono.dev\n\n\nThank you!\nDon't forget to contribute to cocos2d-x\nWithout them this project would not exist.";
            smallAdjustDimensionSystemLabel.Text = text;

            //AddChild(scene);
        }

        public void UpdateLabel(float dt)
        {
            int index = CCRandom.Next(0, fontList.Length - 1);
            var position = new CCPoint(CCDirector.SharedDirector.WinSize.Center.X, CCDirector.SharedDirector.WinSize.Center.Y);
            scene.RemoveChild(japaneseSystemLabel);

            RemoveChild(scene);

            AddChild(scene);

            japaneseSystemLabel = new CCLabel(fontList[index], "Arial", 28)
            {
                Position = position,
                Dimensions = new CCSize(600, 0),
                LineBreakMode = CCTextLineBreakMode.SmartBreak
            };

            scene.AddChild(japaneseSystemLabel);
        }


        public override string title()
        {
            return "SystemFont";
        }

        public override string subtitle()
        {
            return "CCLabel using System Font";
        }
    }
}
