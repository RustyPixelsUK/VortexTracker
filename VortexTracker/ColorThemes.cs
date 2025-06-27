// 
// This is part of Vortex Tracker II project
// 
// (c)2000-2009 S.V.Bulba
// Author: Sergey Bulba, vorobey@mail.khstu.ru
// Support page: http://bulba.untergrund.net/
// 
// Version 2.0 and later
// (c)2017-2019 Ivan Pirog, ivan.pirog@gmail.com
// 
// C# Port by Ben Baker https://baker76.com

using LibVT;
using System.Diagnostics;

namespace VortexTracker
{
    public enum ThemeColor : int
    {
        Background,
        SelLineBackground,
        HighlBackground,
        OutBackground,
        OutHlBackground,
        Text,
        SelLineText,
        HighlText,
        OutText,
        LineNum,
        SelLineNum,
        HighlLineNum,
        Envelope,
        SelEnvelope,
        Noise,
        SelNoise,
        Note,
        SelNote,
        NoteParams,
        SelNoteParams,
        NoteCommands,
        SelNoteCommands,
        Separators,
        OutSeparators,
        SamOrnBackground,
        SamOrnSelBackground,
        SamOrnText,
        SamOrnSelText,
        SamOrnLineNum,
        SamOrnSelLineNum,
        SamNoise,
        SamSelNoise,
        SamOrnSeparators,
        SamOrnTone,
        SamOrnSelTone,
        FullScreenBackground,
        Count
    };

    public enum GridColor : int
    {
        White,
        Red,
        Green,
        Blue,
        Maroon,
        Purple,
        Gray,
        Teal,
        Black,
        Light1,
        Light2,
        Light3,
        Light4,
        Light5,
    };

    public class ColorTheme : ICloneable
    {
        public string Name;
        public Color[] Colors = new Color[(int)ThemeColor.Count];

        public ColorTheme()
        {
        }

        public ColorTheme(string name, string[] colors)
        {
            Name = name;
            for (int i = 0; i < colors.Length; i++)
                Colors[i] = ColorThemes.StringToColor(colors[i]);
        }

        public object Clone()
        {
            ColorTheme clone = new ColorTheme();
            clone.Name = Name;
            clone.Colors = new Color[Colors.Length];
            Array.Copy(Colors, clone.Colors, Colors.Length);
            return clone;
        }
    }

    public class ColorThemes
    {
        public static ColorTheme[] VTColorThemes = new ColorTheme[0]; 
        public static int[] SystemColors = new int[23];
        public const int BrightBG = 0x333333;
        public const int BrightHL = 0x171717;
        public const int BrightTXT = 0x525252;
        public const string ThemeINIKey = "Vortex Tracker 2.0 Theme";
        public static ColorTheme[] DefaultColorThemes =
        {
            new ColorTheme("Default",              new string[] { "FFFFFF", "4256A2", "EFEFEF", "FFFFFF", "F5F5F5", "7D7D88", "798EAB", "54545C", "8D8D95", "454455", "ECEDD1", "414050", "515A6F", "FFFED9", "477C80", "FFFED9", "1F1C65", "FFFED9", "5A5E74", "FFFED9", "536B71", "FFFED9", "8E8E9F", "8E8E9F", "FEFFFA", "4256A2", "766A66", "6D7484", "5C4845", "FFFED9", "766A66", "515A6F", "92929A", "766A66", "515A6F", "221C1C" }),
            new ColorTheme("Peach",                new string[] { "FDFCEF", "393E8C", "FAE7D4", "FBFBE8", "EFEFCC", "AC8576", "B7B85C", "8F8B5B", "C18573", "915D57", "ECEDD1", "6B6844", "5E4C45", "E1E4C8", "A4744E", "E1E4C8", "00219D", "E1E4C8", "7F665D", "E1E4C8", "765F57", "E1E4C8", "D9998D", "B3B3A8", "FCFCEC", "424584", "926858", "714F44", "915D57", "F6F5D3", "915F5C", "63326A", "D9998D", "604D47", "24546E", "221C1C"  }),
            new ColorTheme("Classic Vortex 1.0",   new string[] { "FFFFFF", "0078D7", "EFEFEF", "FEFFFA", "F3F8FE", "000000", "FFFFFF", "000000", "828282", "000000", "FFFFFF", "000000", "000000", "FFFFFF", "000000", "FFFFFF", "000000", "FFFFFF", "000000", "FFFFFF", "000000", "FFFFFF", "404040", "404040", "FFFFFF", "006DC3", "626262", "686868", "252525", "FFFFFF", "626262", "3F3D56", "727272", "626262", "3F3D56", "221C1C" }),
            new ColorTheme("MmcM",                 new string[] { "000000", "0009A5", "000000", "000000", "000000", "808080", "CCCCCC", "D0D0D0", "808080", "808080", "C0C0C0", "D0D0D0", "06A6E1", "CCCCCC", "D47908", "CCCCCC", "CCCCCC", "CCCCCC", "00A30C", "CCCCCC", "C0C000", "CCCCCC", "505050", "505050", "000000", "0009A5", "CCCCCC", "C6C6C6", "808080", "D4D4D4", "EE8809", "EE8809", "606060", "59C40C", "59C40C", "001020" }),
            new ColorTheme("n1k-o",                new string[] { "000000", "0009A5", "1D1D1D", "000000", "181818", "777777", "CCCCCC", "DAA721", "636363", "7F7F7F", "CCCCCC", "BBBBBB", "06A6E1", "CCCCCC", "D47908", "CCCCCC", "CCCCCC", "CCCCCC", "00A30C", "CCCCCC", "C9C804", "CCCCCC", "717171", "3C3C3C", "000000", "0009A5", "CCCCCC", "C6C6C6", "7B7B7B", "D4D4D4", "EE8809", "EE8809", "717171", "59C40C", "59C40C", "001020" }),
            new ColorTheme("EA's Theme",           new string[] { "000000", "0000FF", "000040", "000000", "000040", "404040", "FFFFFF", "404040", "404040", "008000", "FFFFFF", "00B501", "00FFFF", "FFFFFF", "FF8000", "FFFFFF", "FFFFFF", "FFFFFF", "00FF00", "FFFFFF", "FFFF00", "FFFFFF", "C0C0C0", "404040", "000000", "FF0000", "808080", "FFFFFF", "008000", "EEF41F", "C08000", "FF8000", "C0C0C0", "008000", "00FF00", "000000" }),
            new ColorTheme("Flexx",                new string[] { "131313", "42387A", "1D1C18", "161616", "1E1D19", "00570C", "B3B3B3", "515152", "4E4A35", "3F6B93", "FFFFFF", "44739E", "FA7351", "DAD9E4", "D3CCB2", "D8D2BB", "9391F4", "EEEEC8", "BF6240", "E8AB95", "35A36D", "A1CBB2", "4B4D44", "3B3D35", "131313", "3E306D", "B4B591", "BFB398", "3F675D", "CFDA9C", "8584A8", "9CABCA", "5E6156", "C77759", "D8C075", "0C0C0C" }),
            new ColorTheme("FruityLoops",          new string[] { "34444E", "29363D", "3D4E59", "3A4853", "3E4F5D", "4D565B", "CFDEEA", "808082", "607783", "7CA0AA", "98B4BC", "6F97A2", "8590AE", "778BB1", "8C98BB", "778BB1", "C6C8CB", "D8E8F1", "77808A", "B997AC", "5C6E70", "A1CBB2", "5F686D", "5F686D", "34444E", "526878", "8DACB4", "CFDEEA", "7CA0AA", "B9D0D0", "6B799F", "7A8CAD", "5F686D", "848C95", "C3A6B8", "34444E" }),
            new ColorTheme("OpenMPT",              new string[] { "FAFAFA", "2B2B2B", "EEEEEE", "FAFAFA", "F3F3F3", "8A8A8A", "FFFFFF", "757575", "A5A5A5", "374D5B", "FFFFFF", "1F2B33", "714D47", "FFFFFF", "617B4C", "FFFFFF", "38386E", "FFFFFF", "458C3B", "FFFFFF", "86880A", "FFFFFF", "CBCBCB", "C7C7BF", "FAFAFA", "3C3C3C", "8A8A8A", "747474", "40596A", "BFD8D1", "458C3B", "7B750F", "A9A99D", "565686", "57577E", "607179" }),
            new ColorTheme("Relaxed",              new string[] { "111B21", "275272", "122832", "131E24", "142C37", "303D43", "B7B85C", "4E626C", "3B584C", "A2B099", "ECEDD1", "BAC4B3", "8D8942", "E1E4C8", "997D59", "E1E4C8", "F3A586", "E1E4C8", "8E8D5E", "E1E4C8", "8E8D5E", "E1E4C8", "35434A", "35434A", "131E24", "1A326D", "C0B77F", "CBC395", "A2B099", "F6F5D3", "9F9D6D", "D5D4B3", "35434A", "9F9D6D", "DEDDAD", "0F0D0D" }),
            new ColorTheme("Quiet",                new string[] { "181B18", "17BCD9", "282828", "191919", "262626", "10991C", "FFFFFF", "515152", "717171", "448265", "FFFFFF", "55A25E", "16DA28", "FFFFFF", "EFF518", "FFFFFF", "16DA28", "FFFFFF", "76D2E2", "FFFFFF", "B65FFC", "FFFFFF", "31BDC1", "434343", "181B18", "2C2C2C", "63A569", "85A889", "3F675D", "7BC6B8", "A9AC3A", "D3D74D", "256462", "9270AE", "AF88CF", "141614" }),
            new ColorTheme("Bfox",                 new string[] { "000000", "0000E6", "101010", "000000", "0F0F0F", "005000", "FEFEFE", "005000", "303030", "00CC00", "FEFEFE", "009D00", "009D00", "FEFEFE", "009D00", "FEFEFE", "00CC00", "FEFEFE", "009D00", "FEFEFE", "009D00", "FEFEFE", "9D9D9D", "202020", "000000", "7840CC", "CCCCCC", "E9E9E9", "00A8A8", "CCCCCC", "CCCC00", "E9E900", "9D9D9D", "CCCCCC", "FEFEFE", "030403" }),
            new ColorTheme("PT2 Scheme",           new string[] { "000000", "000000", "181818", "000000", "0F0F0F", "007800", "CCCCCC", "007800", "303030", "00CC00", "FEFEFE", "CCCC00", "00CCCC", "FEFEFE", "CCCC00", "FEFEFE", "00CC00", "FEFEFE", "00CC00", "FEFEFE", "00CC00", "FEFEFE", "CCCCCC", "3A3A3A", "000000", "CCCCCC", "CCCCCC", "E9E9E9", "00CC00", "000000", "CCCC00", "E9E900", "787878", "00CCCC", "00E9E9", "090909" }),
            new ColorTheme("PT3 Green",            new string[] { "000000", "0000FE", "141414", "000000", "0F0F0F", "009D00", "FEFEFE", "009D00", "383838", "00FE00", "FEFEFE", "00CC00", "00CC00", "FEFEFE", "00CC00", "FEFEFE", "00CC00", "FEFEFE", "00CC00", "FEFEFE", "00CC00", "FEFEFE", "CCCCCC", "3A3A3A", "000000", "CC00CC", "CCCCCC", "CCCCCC", "00CCCC", "CCCCCC", "CCCCCC", "CCCCCC", "CCCCCC", "CCCCCC", "CCCCCC", "090909" }),
            new ColorTheme("Calmness",             new string[] { "181F2D", "1E4769", "1A2635", "181F2D", "192433", "3F3852", "918581", "464F54", "3D3D3D", "769A93", "DFDFDF", "7BA39A", "816E88", "B4B4B4", "7E6049", "B6B6B6", "D5D8CA", "FFFFFF", "806E4F", "CACACA", "8E6B47", "CACACA", "2D424E", "2D424E", "181F2D", "1E4769", "9EABA0", "B7C4BB", "80A19A", "DFDFDF", "588150", "6B9962", "2D424E", "8F614F", "B38575", "12121C" }),
            new ColorTheme("Grayscaled Light",     new string[] { "FFFFFF", "666666", "EFEFEF", "FFFFFF", "F9F9F9", "929292", "CCCCCC", "4F595F", "B9B9B9", "6C6C6C", "DFDFDF", "475056", "606060", "E0E0E0", "585858", "F7F7F7", "383838", "FFFFFF", "696969", "EAEAEA", "7C7C7C", "D2D2D2", "909090", "ACACAC", "FFFFFF", "787878", "6F6F6F", "636363", "7E7E7E", "ECECEC", "484848", "3C3C3C", "909090", "5C5C5C", "474747", "737373" }),
            new ColorTheme("Grayscaled Dark",      new string[] { "131313", "3B3B3B", "191919", "0B0B0B", "1A1A1A", "323232", "CCCCCC", "424B50", "323232", "515151", "DFDFDF", "7C7C7C", "606060", "B4B4B4", "585858", "B6B6B6", "C5C5C5", "FFFFFF", "696969", "CACACA", "565656", "CACACA", "3A3A3A", "353535", "131313", "393939", "A1A1A1", "8B8B8B", "BABABA", "909090", "B1B1B1", "BFBFBF", "4E4E4E", "9E9E9E", "BABABA", "101010" }),
            new ColorTheme("Dark Blue",            new string[] { "101021", "242478", "181830", "0D0D1B", "141427", "2C2739", "918581", "424B50", "364357", "63494D", "DFDFDF", "906B70", "507088", "B4B4B4", "4C7A67", "B6B6B6", "CCCCD3", "FFFFFF", "6A463A", "CACACA", "734D69", "CACACA", "242139", "1E1B2F", "101021", "1C1C36", "837F90", "A0A2B1", "63494D", "8E696E", "4F7448", "6B9962", "3F3B58", "764F41", "A36E5A", "0B0B1A" }),
            new ColorTheme("Burgundi",             new string[] { "211010", "772222", "281414", "180B0B", "1D1010", "392927", "8D5E4D", "433737", "3F251E", "914237", "AED697", "C16F64", "81814B", "B4B4B4", "756537", "B89878", "E8C64D", "E9C54C", "AA654B", "CB8F79", "4F6C3D", "83B761", "402525", "361E1E", "211010", "611919", "B68774", "C5A094", "773C34", "C1A173", "AA654B", "BE6C4E", "3D2323", "AE7F43", "C38940", "120D0D" }),
            new ColorTheme("Night Mode",           new string[] { "060906", "193449", "0B1315", "060906", "080E10", "172825", "8D5E4D", "283431", "192329", "1F4240", "2B706F", "326A67", "896241", "B8875D", "756537", "B89878", "E7C731", "E9C54C", "476B32", "5B8A40", "543B31", "8D6453", "1A2B2D", "152324", "060906", "432718", "62AD6B", "71C77F", "4B4F25", "AB8E4D", "AA654B", "BE6C4E", "1A2B2D", "697530", "6CA04A", "060906" }),
            new ColorTheme("Night Mode 2",         new string[] { "0B0C1B", "521C1C", "161627", "080911", "12121B", "172825", "8D5E4D", "283431", "443727", "8C7545", "C7C870", "B69E6C", "7792A6", "91A8B9", "786838", "B89878", "E0E542", "EDCF69", "6F8B42", "947A46", "A97049", "A97049", "4D302A", "2D2423", "0B0C1B", "521C1C", "AC7D6C", "CAC34D", "733838", "BFB756", "B5B92A", "DDE32D", "4D302A", "B9662E", "D48048", "060906" }),
            new ColorTheme("School Notebook",      new string[] { "FFFFFF", "414273", "ECF1E4", "FFFFFF", "F8FAF5", "8E8F9F", "7175B1", "767996", "918FAF", "7A7995", "E5E9F5", "56566C", "B26853", "F6DAC3", "756537", "B89878", "4E4ACB", "FCFCF2", "A5693D", "D8B296", "A75B83", "E8B7D1", "BAB7DB", "C0BEDE", "FFFFFF", "FFFE8B", "62A4C3", "519ABC", "658DB6", "337FCE", "B26853", "CD421B", "BAB7DB", "6866A8", "615FAE", "3D3D5D" }),
            new ColorTheme("Iodine",               new string[] { "F7F7E7", "7D5737", "FFEBD2", "F7F7E7", "EEF1DE", "AB9F60", "B4A971", "9D9C54", "B6A573", "804D38", "FFFAFA", "6A402E", "737046", "FBF5E3", "617544", "FBF0EA", "29573B", "FBF0EA", "927553", "FBF0EA", "3F8F38", "FBF0EA", "BF9E75", "C6A884", "F7F7E7", "B0884D", "B09172", "917F42", "50965A", "FDFFE1", "3C8F70", "437F68", "C29E5E", "AD823E", "A95119", "181412" }),
            new ColorTheme("Aqua",                 new string[] { "0B4152", "3A84BA", "3C4F68", "18415B", "0E4F64", "098285", "FFFFFF", "0A989B", "208D9E", "9DB7C8", "FFFFFF", "A9C0CF", "00F782", "FFFFFF", "EBB54D", "FFFFFF", "E9FFFF", "FFFFFF", "5EE566", "FFFFFF", "CBCA1A", "FFFFFF", "5B86A3", "5B86A3", "0C4A5D", "0865E0", "9AEFF8", "FFFFFF", "9DB7C8", "DDFADC", "EBB54D", "EEC456", "DFDFDF", "FFFFE7", "FFFEA5", "072C38" }),
            new ColorTheme("Red Wine",             new string[] { "290D0D", "911E1E", "350D0D", "280C1B", "31141C", "803030", "ADBA5F", "973939", "6E402D", "CE6262", "FCF9B2", "F88181", "BFDE40", "FCF9B2", "E6D689", "FCF9B2", "D1D400", "FFFD9C", "C28342", "FFFD9C", "BF841A", "FFFD9C", "431D13", "431D13", "2D0E0E", "892B2B", "AB9D74", "C1C298", "B35F5F", "FDFCD2", "E69746", "E3AC75", "431D13", "97BE80", "BDD799", "1D0B1E" }),
            new ColorTheme("Green State",          new string[] { "132416", "1B5E16", "1D331F", "081D13", "172A19", "627A5F", "7CD568", "586D54", "3D5B28", "AEAB6B", "FEFEEC", "B4E19C", "7CDE40", "FEFEEC", "E6CB89", "FEFEEC", "B7F2B2", "FEFEEC", "C9A84B", "FEFEEC", "C2C123", "FEFEEC", "37663A", "37663A", "182D1B", "26681A", "B7F2B2", "EEEBC0", "9CF8BB", "F9E5AA", "FCBE64", "FEE3BA", "38693B", "FDFC73", "C1EC80", "072C38" }),
            new ColorTheme("Norton",               new string[] { "181F3B", "414186", "232B4E", "0D1536", "192042", "103E59", "399FB6", "355777", "455855", "9C9F7C", "ECEDD1", "C9CB9D", "B5B887", "E0DE92", "847D42", "E0DE92", "FBFBD8", "F4F2A7", "D29673", "E0DE92", "BA7E5A", "E0DE92", "1D3656", "1D3656", "181F3B", "3612A3", "D1D3A9", "F9F7B7", "C9CB9D", "ECEDD1", "D5B380", "E4B771", "0B4569", "D0AB71", "EDC079", "121022" })
        };

        public static string[,] WinColorThemes = new string[,]
        {
            { "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000" },
            { "C8D0D4", "E6E3E1", "2D2319", "7D5F3C", "E6E3E1", "F0F0F0", "000000", "2D2319", "321900", "321900", "808080", "735532", "FFF5EB", "E6E3E1", "D2CFCD", "000000", "FAF7F5", "F5EBE1", "AAA096", "C8C5C3", "7D5F3C", "000000", "E1FFFF" },
            { "C8D0D4", "E5EDF0", "000000", "958370", "E5EDF0", "FBFBFB", "000000", "000000", "C8D0D4", "C8D0D4", "808080", "958370", "F5F5F5", "E5EDF0", "C2CDD4", "000000", "FAFAFA", "E9E9E9", "969696", "C2CDD4", "64C8C0", "000000", "E0EDEE" },
            { "C8D0D4", "E8E8E8", "000000", "967A55", "E8E8E8", "FFFFFF", "000000", "000000", "C8D0D4", "C8D0D4", "808080", "967A55", "FFFFFF", "E8E8E8", "C8C8C8", "000000", "E8E8E8", "EBEBEB", "C8C8C8", "A0A0A0", "B7B7B7", "000000", "E8E8E8" },
            { "C8D0D4", "FFFFFF", "4B4B4B", "EA9A3B", "F3EFEF", "FFFFFF", "000000", "000000", "C8D0D4", "C8D0D4", "808080", "DD7E26", "FFFFFF", "F3EFEF", "ACA899", "000000", "FFFFFF", "E2EFF1", "99A8AC", "C8C5C3", "800000", "000000", "E8E8E8" },
            { "C8D0D4", "DCDCDC", "000000", "926836", "DCDCDC", "FFFFFF", "000000", "000000", "C8D0D4", "C8D0D4", "808080", "926836", "FFFFFF", "DCDCDC", "B4B4B4", "000000", "DCDCDC", "EBEBEB", "B4B4B4", "A0A0A0", "B7B7B7", "000000", "DCDCDC" },
            { "D0D0D0", "FFFFFF", "000000", "9A785C", "FFFFFF", "FFFFFF", "000000", "000000", "D0D0D0", "D0D0D0", "808080", "99705E", "FFFFFF", "FFFFFF", "C2CDD4", "000000", "FFFFFF", "E2EFF1", "99A8AC", "C2CDD4", "800000", "000000", "FFFFFF" }
        };

        public static string[] GridColors = new string[]
        {
            "FFFFFF", "BA2525", "30833A", "3E4895", "A55829", "994891", "727272", "0E8C8F", "000000", "FF8C8C", "FFCDCD", "FAFFFF", "B7FFB9", "C6C6FF"
        };

        private static Win32.COLOR[] WinColors = new Win32.COLOR[]
        {
            Win32.COLOR.SCROLLBAR,
            Win32.COLOR.MENU,
            Win32.COLOR.MENUTEXT,
            Win32.COLOR.MENUHILIGHT,
            Win32.COLOR.MENUBAR,
            Win32.COLOR.WINDOW,
            Win32.COLOR.WINDOWFRAME,
            Win32.COLOR.WINDOWTEXT,
            Win32.COLOR.ACTIVEBORDER,
            Win32.COLOR.INACTIVEBORDER,
            Win32.COLOR.APPWORKSPACE,
            Win32.COLOR.HIGHLIGHT,
            Win32.COLOR.HIGHLIGHTTEXT,
            Win32.COLOR.BTNFACE,
            Win32.COLOR.BTNSHADOW,
            Win32.COLOR.BTNTEXT,
            Win32.COLOR.BTNHIGHLIGHT,
            Win32.COLOR.THREEDLIGHT,
            Win32.COLOR.GRAYTEXT,
            Win32.COLOR.THREEDDKSHADOW,
            Win32.COLOR.HOTLIGHT,
            Win32.COLOR.INFOTEXT,
            Win32.COLOR.INFOBK
        };

        static ColorThemes()
        {
            InitColorThemes();
        }

        public static Color ChangeBrightness(Color color, int shift)
        {
            Color result;
            int r = color.R;
            int g = color.G;
            int b = color.B;
            r = Math.Clamp(r + shift, 0, 255);
            g = Math.Clamp(g + shift, 0, 255);
            b = Math.Clamp(b + shift, 0, 255); 
            result = Color.FromArgb(r, g, b);
            return result;
        }

        public static Color ChangeBlueColor(Color color, int shift)
        {
            Color result;
            int r = color.R;
            int g = color.G;
            int b = color.B;
            b = Math.Clamp(b + shift, 0, 255);
            result = Color.FromArgb(r, g, b);
            return result;
        }

        public static Color ChangeRedColor(Color color, int shift)
        {
            Color result;
            int r = color.R;
            int g = color.G;
            int b = color.B;
            r = Math.Clamp(r + shift, 0, 255);
            result = Color.FromArgb(r, g, b);
            return result;
        }

        public static Color ChangeGreenColor(Color color, int shift)
        {
            Color result;
            int r = color.R;
            int g = color.G;
            int b = color.B;
            g = Math.Clamp(g + shift, 0, 255);
            result = Color.FromArgb(r, g, b);
            return result;
        }

        public static Color GetSelectionColor(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int r1 = r, g1 = g, b1 = b;

            r = Math.Clamp(r - 30, 40, 255);
            g = Math.Clamp(g - 30, 40, 255);
            b = Math.Clamp(b - 30, 40, 255);

            if (b1 < r1 && b1 < g1)
                b += 30;
            else if (r1 < b1 && r1 < g1)
                r += 30;
            else if (g1 < b1 && g1 < r1)
                g += 30;
            else
                b += 30;

            r = Math.Clamp(r, 40, 200);
            g = Math.Clamp(g, 40, 200);
            b = Math.Clamp(b, 40, 200);

            return Color.FromArgb(r, g, b);
        }

        public static Color GetHighlightColor(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int r1 = r, g1 = g, b1 = b;

            r = Math.Clamp(r + 15, 0, 255);
            g = Math.Clamp(g + 15, 0, 255);
            b = Math.Clamp(b + 15, 0, 255);

            if (b1 < r1 && b1 < g1)
                b += 10;
            else if (r1 < b1 && r1 < g1)
                r += 10;
            else if (g1 < b1 && g1 < r1)
                g += 10;

            r = Math.Clamp(r, 40, 200);
            g = Math.Clamp(g, 40, 200);
            b = Math.Clamp(b, 40, 200);

            return Color.FromArgb(r, g, b);
        }

        public static void SetupColorBars(ColorTheme theme)
        {
            if (Globals.OptionsForm == null)
                return;

            Globals.OptionsForm.ColBackground.BackColor = theme.Colors[(int)ThemeColor.Background];
            Globals.OptionsForm.ColSelLineBackground.BackColor = theme.Colors[(int)ThemeColor.SelLineBackground];
            Globals.OptionsForm.ColHighlBackground.BackColor = theme.Colors[(int)ThemeColor.HighlBackground];
            Globals.OptionsForm.ColOutBackground.BackColor = theme.Colors[(int)ThemeColor.OutBackground];
            Globals.OptionsForm.ColOutHlBackground.BackColor = theme.Colors[(int)ThemeColor.OutHlBackground];
            Globals.OptionsForm.ColText.BackColor = theme.Colors[(int)ThemeColor.Text];
            Globals.OptionsForm.ColSelLineText.BackColor = theme.Colors[(int)ThemeColor.SelLineText];
            Globals.OptionsForm.ColHighlText.BackColor = theme.Colors[(int)ThemeColor.HighlText];
            Globals.OptionsForm.ColOutText.BackColor = theme.Colors[(int)ThemeColor.OutText];
            Globals.OptionsForm.ColLineNum.BackColor = theme.Colors[(int)ThemeColor.LineNum];
            Globals.OptionsForm.ColSelLineNum.BackColor = theme.Colors[(int)ThemeColor.SelLineNum];
            Globals.OptionsForm.ColHighlLineNum.BackColor = theme.Colors[(int)ThemeColor.HighlLineNum];
            Globals.OptionsForm.ColEnvelope.BackColor = theme.Colors[(int)ThemeColor.Envelope];
            Globals.OptionsForm.ColSelEnvelope.BackColor = theme.Colors[(int)ThemeColor.SelEnvelope];
            Globals.OptionsForm.ColNoise.BackColor = theme.Colors[(int)ThemeColor.Noise];
            Globals.OptionsForm.ColSelNoise.BackColor = theme.Colors[(int)ThemeColor.SelNoise];
            Globals.OptionsForm.ColNote.BackColor = theme.Colors[(int)ThemeColor.Note];
            Globals.OptionsForm.ColSelNote.BackColor = theme.Colors[(int)ThemeColor.SelNote];
            Globals.OptionsForm.ColNoteParams.BackColor = theme.Colors[(int)ThemeColor.NoteParams];
            Globals.OptionsForm.ColSelNoteParams.BackColor = theme.Colors[(int)ThemeColor.SelNoteParams];
            Globals.OptionsForm.ColNoteCommands.BackColor = theme.Colors[(int)ThemeColor.NoteCommands];
            Globals.OptionsForm.ColSelNoteCommands.BackColor = theme.Colors[(int)ThemeColor.SelNoteCommands];
            Globals.OptionsForm.ColSeparators.BackColor = theme.Colors[(int)ThemeColor.Separators];
            Globals.OptionsForm.ColOutSeparators.BackColor = theme.Colors[(int)ThemeColor.OutSeparators];
            Globals.OptionsForm.ColSamOrnBackground.BackColor = theme.Colors[(int)ThemeColor.SamOrnBackground];
            Globals.OptionsForm.ColSamOrnSelBackground.BackColor = theme.Colors[(int)ThemeColor.SamOrnSelBackground];
            Globals.OptionsForm.ColSamOrnText.BackColor = theme.Colors[(int)ThemeColor.SamOrnText];
            Globals.OptionsForm.ColSamOrnSelText.BackColor = theme.Colors[(int)ThemeColor.SamOrnSelText];
            Globals.OptionsForm.ColSamOrnLineNum.BackColor = theme.Colors[(int)ThemeColor.SamOrnLineNum];
            Globals.OptionsForm.ColSamOrnSelLineNum.BackColor = theme.Colors[(int)ThemeColor.SamOrnSelLineNum];
            Globals.OptionsForm.ColSamNoise.BackColor = theme.Colors[(int)ThemeColor.SamNoise];
            Globals.OptionsForm.ColSamSelNoise.BackColor = theme.Colors[(int)ThemeColor.SamSelNoise];
            Globals.OptionsForm.ColSamOrnSeparators.BackColor = theme.Colors[(int)ThemeColor.SamOrnSeparators];
            Globals.OptionsForm.ColSamOrnTone.BackColor = theme.Colors[(int)ThemeColor.SamOrnTone];
            Globals.OptionsForm.ColSamOrnSelTone.BackColor = theme.Colors[(int)ThemeColor.SamOrnSelTone];
            Globals.OptionsForm.ColFullScreenBackground.BackColor = theme.Colors[(int)ThemeColor.FullScreenBackground];
        }

        public static string SelectedThemeName()
        {
            for (int i = 0; i < Globals.OptionsForm.ColorThemesList.Items.Count; i++)
            {
                if (Globals.OptionsForm.ColorThemesList.GetSelected(i))
                    return Globals.OptionsForm.ColorThemesList.Items[i].ToString();
            }

            return "";
        }

        public static int GetThemeIndex(string themeName)
        {
            for (int i = 0; i < VTColorThemes.Length; i++)
            {
                if (VTColorThemes[i].Name == themeName)
                    return i;
            }

            return -1;
        }

        public static bool ColorThemeExists(string ThemeName)
        {
            for (int i = 0; i < VTColorThemes.Length; i++)
            {
                if (VTColorThemes[i].Name == ThemeName)
                    return true;
            }

            return false;
        }

        public static bool ValidColorThemeName(string newName)
        {
            newName = newName.Trim();

            if (newName == "")
                return false;

            return !ColorThemeExists(newName);
        }

        public static void AddColorTheme(ColorTheme theme)
        {
            // Increase themes array length
            VTColorThemes = new ColorTheme[VTColorThemes.Length + 1];
 
            // Shift themes
            for (int i = VTColorThemes.Length - 1; i >= 1; i--)
                VTColorThemes[i] = VTColorThemes[i - 1];

            // Insert new theme at first position
            VTColorThemes[0] = theme;
            MainForm.ColorTheme = theme;
            Globals.MainForm.PrepareColors();
            FillColorThemesList();
        }

        public static void SetColorTheme(ColorTheme theme)
        {
            bool redraw = false;

            if (MainForm.ColorThemeName != theme.Name)
                redraw = true;

            MainForm.ColorThemeName = theme.Name;
            MainForm.ColorTheme = theme;

            Globals.MainForm.PrepareColors();

            if (Globals.OptionsForm != null)
            {
                int i = GetThemeIndex(theme.Name);
                Globals.OptionsForm.ColorThemesList.SetSelected(i, true);
                SetupColorBars(theme);
            }

            if (redraw && !MainForm.SyncVTInstanses)
                Globals.MainForm.RedrawChilds();
        }

        public static void SetColorThemeByName(string themeName)
        {
            ColorTheme theme = GetColorTheme(themeName);
            SetColorTheme(theme);
        }

        public static void InitColorThemes()
        {
            if (VTColorThemes.Length == 0)
            {
                VTColorThemes = new ColorTheme[DefaultColorThemes.Length];

                for (int i = 0; i < DefaultColorThemes.Length; i++)
                    VTColorThemes[i] = (ColorTheme)DefaultColorThemes[i].Clone();
            }

            FillColorThemesList();

            if (GetThemeIndex(MainForm.ColorThemeName) ==  -1)
                SetColorTheme(VTColorThemes[0]);
            else
                SetColorThemeByName(MainForm.ColorThemeName);
        }

        public static void FillColorThemesList()
        {
            int i;
            if (Globals.OptionsForm == null)
                return;

            Globals.OptionsForm.ColorThemesList.Items.Clear();

            for (i = 0; i < VTColorThemes.Length; i ++)
            {
                Globals.OptionsForm.ColorThemesList.Items.Add(VTColorThemes[i].Name);

                if (VTColorThemes[i].Name == MainForm.ColorThemeName)
                    Globals.OptionsForm.ColorThemesList.SetSelected(i, true);
            }

            i = GetThemeIndex(MainForm.ColorThemeName);

            SetColorTheme(VTColorThemes[i == -1 ? 0 : i]);
            // Form1.BtnRenameTheme.Enabled := Form1.BtnSaveTheme.Enabled;
            Globals.OptionsForm.DeleteThemeButton.Enabled = VTColorThemes.Length > 1;
        }

        public static void UpdateCurrentTheme()
        {
            ColorTheme theme = GetCurrentColorTheme();
            int i = GetThemeIndex(theme.Name);
            VTColorThemes[i] = MainForm.ColorTheme;
        }

        public static void SaveColorTheme(string fileName, string themeName)
        {
            System.IO.Stream ini;
            ColorTheme theme = GetColorTheme(themeName);
            IniFile iniFile = new IniFile(fileName);

            iniFile.SetValue(ThemeINIKey, "Name", themeName);

            for (int i = 0; i < (int)ThemeColor.Count; i++)
                iniFile.SetValue(ThemeINIKey, ((ThemeColor)i).ToString(), theme.Colors[i]);
        }

        public static bool LoadColorTheme(string fileName)
        {
            bool result = true;
            ColorTheme theme = new ColorTheme();
            string newName;
            IniFile iniFile = new IniFile(fileName);

            theme.Name = iniFile.GetValue(ThemeINIKey, "Name", "");

            if (theme.Name == "")
                return false;

            for (int i = 0; i < (int)ThemeColor.Count; i++)
                theme.Colors[i] = iniFile.GetValue(ThemeINIKey, ((ThemeColor)i).ToString(), MainForm.ThemeColors[i]);
        
            if (ColorThemeExists(theme.Name))
            {
                int i = 1;
                do
                {
                    newName = $"{theme.Name} {i++}";
                }
                while (ColorThemeExists(newName));

                theme.Name = newName;
            }

            AddColorTheme(theme);
            SetColorTheme(theme);

            return result;
        }

        public static void CloneColorTheme()
        {
            string newName = "";
            ColorTheme theme = GetCurrentColorTheme();
            do
            {
                if (!Globals.InputQuery(Application.ProductName, "Enter new theme name", ref newName))
                    return;
            }
            while (!ValidColorThemeName(newName));

            theme.Name = newName;
            AddColorTheme(theme);
            SetColorTheme(theme);
        }

        public static Color GetColor(string color)
        {
            return ColorTranslator.FromHtml("#" + color);
        }

        public static ColorTheme GetColorTheme(string themeName)
        {
            for (int i = 0; i < VTColorThemes.Length; i++)
            {
                if (VTColorThemes[i].Name == themeName)
                    return VTColorThemes[i];
            }

            return VTColorThemes[0];
        }

        public static ColorTheme GetCurrentColorTheme()
        {
            ColorTheme result = GetColorTheme("Default");
            string themeName = SelectedThemeName();

            for (int i = 0; i < Globals.OptionsForm.ColorThemesList.Items.Count; i ++)
            {
                if (Globals.OptionsForm.ColorThemesList.Items[i].ToString() == themeName)
                    return VTColorThemes[i];
            }

            return result;
        }

        public static void RenameSelectedTheme()
        {
            int i = GetThemeIndex(SelectedThemeName());
            string newName = VTColorThemes[i].Name;

            do
            {
                if (!Globals.InputQuery(Application.ProductName, "Enter a new name", ref newName))
                    return;
            }
            while (!ValidColorThemeName(newName));

            VTColorThemes[i].Name = newName;
            MainForm.ColorThemeName = newName;
            FillColorThemesList();
        }

        public static void DeleteSelectedTheme()
        {
            ColorTheme theme = GetCurrentColorTheme();

            if (MessageBox.Show(Globals.MainForm, "Are you sure?", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
                return;

            // Shift themes to right
            for (int i = GetThemeIndex(theme.Name); i < VTColorThemes.Length - 1; i++)
                VTColorThemes[i] = VTColorThemes[i + 1];

            // Decrease themes array
            VTColorThemes = new ColorTheme[VTColorThemes.Length - 1];
            SetColorTheme(VTColorThemes[0]);
            FillColorThemesList();
        }

        public static ColorTheme LoadColorThemeFromStr(string name, string colorsString)
        {
            ColorTheme result = (ColorTheme)DefaultColorThemes[0].Clone();

            result.Name = name;

            string[] colors = colorsString.Split(',');

            for (int i = 0; i < colors.Length; i++)
                result.Colors[i] = StringToColor(colors[i]);

            return result;
        }

        public static Color StringToColor(string color)
        {
            return Color.FromArgb(255, Color.FromArgb(Convert.ToInt32(color, 16)));
        }

        public static string ColorToString(Color color)
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Color ColorFromSysColor(Win32.COLOR color)
        {
            int rgb = (int)Win32.GetSysColor(color);
            byte r = (byte)(rgb & 0xFF);
            byte g = (byte)((rgb & 0xFF00) >> 8);
            byte b = (byte)((rgb & 0xFF0000) >> 16);
            return Color.FromArgb(r, g, b);
        }

        public static void SaveSystemColors()
        {
            for (int i = 0; i < WinColors.Length; i++)
            {
                SystemColors[i] = (int)Win32.GetSysColor(WinColors[i]);
                WinColorThemes[0, i] = ColorToString(ColorFromSysColor(WinColors[i]));
            }
        }

        public static void RestoreSystemColors()
        {
            int[] elements = Array.ConvertAll(WinColors, w => (int)w);
            Win32.SetSysColors(SystemColors.Length, elements, SystemColors);
        }

        public static void SetWindowColors(int themeIndex)
        {
            int[] elements = Array.ConvertAll(WinColors, w => (int)w);
            int[] colors = new int[(int)ThemeColor.Count];

            for (int i = 0; i < WinColors.Length; i++)
            {
                Color color = StringToColor(WinColorThemes[themeIndex, i]);
                colors[i] = (color.R << 16) | (color.G << 8) | color.B;
            }

            Win32.SetSysColors(colors.Length, elements, colors);
        }
    }
}