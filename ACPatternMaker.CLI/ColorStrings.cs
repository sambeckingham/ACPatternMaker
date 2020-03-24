using System.Collections.Generic;

namespace ACPatternMaker
{
    internal static class ColorStrings
    {
        public static Dictionary<string, string> Dictionary
        {
            get
            {
                var colorStrings = new Dictionary<string, string>
                {
                    {"#FFEFFFFF", "00"},
                    {"#FF9AADFF", "01"},
                    {"#EF559CFF", "02"},
                    {"#FF65ADFF", "03"},
                    {"#FF0063FF", "04"},
                    {"#BD4573FF", "05"},
                    {"#CE0052FF", "06"},
                    {"#9C0031FF", "07"},
                    {"#522031FF", "08"},

                    //reds
                    {"#FFBACEFF", "10"},
                    {"#FF7573FF", "11"},
                    {"#DE3010FF", "12"},
                    {"#FF5542FF", "13"},
                    {"#FF0000FF", "14"},
                    {"#CE6563FF", "15"},
                    {"#BD4542FF", "16"},
                    {"#BD0000FF", "17"},
                    {"#8C2021FF", "18"},

                    //oranges
                    {"#DECFBDFF", "20"},
                    {"#FFCF63FF", "21"},
                    {"#DE6521FF", "22"},
                    {"#FFAA21FF", "23"},
                    {"#FF6500FF", "24"},
                    {"#BD8A52FF", "25"},
                    {"#DE4500FF", "26"},
                    {"#BD4500FF", "27"},
                    {"#633010FF", "28"},

                    //pastels or something, I guess?
                    {"#FFEFDEFF", "30"},
                    {"#FFDFCEFF", "31"},
                    {"#FFCFADFF", "32"},
                    {"#FFBA8CFF", "33"},
                    {"#FFAA8CFF", "34"},
                    {"#DE8A63FF", "35"},
                    {"#BD6542FF", "36"},
                    {"#9C5531FF", "37"},
                    {"#8C4521FF", "38"},

                    //purple
                    {"#FFCFFFFF", "40"},
                    {"#EF8AFFFF", "41"},
                    {"#CE65DEFF", "42"},
                    {"#BD8ACEFF", "43"},
                    {"#CE00FFFF", "44"},
                    {"#9C659CFF", "45"},
                    {"#8C00ADFF", "46"},
                    {"#520073FF", "47"},
                    {"#310042FF", "48"},

                    //pink
                    {"#FFBAFFFF", "50"},
                    {"#FF9AFFFF", "51"},
                    {"#DE20BDFF", "52"},
                    {"#FF55EFFF", "53"},
                    {"#FF00CEFF", "54"},
                    {"#8C5573FF", "55"},
                    {"#BD009CFF", "56"},
                    {"#8C0063FF", "57"},
                    {"#520042FF", "58"},

                    //brown
                    {"#DEBA9CFF", "60"},
                    {"#CEAA73FF", "61"},
                    {"#734531FF", "62"},
                    {"#AD7542FF", "63"},
                    {"#9C3000FF", "64"},
                    {"#733021FF", "65"},
                    {"#522000FF", "66"},
                    {"#311000FF", "67"},
                    {"#211000FF", "68"},

                    //yellow
                    {"#FFFFCEFF", "70"},
                    {"#FFFF73FF", "71"},
                    {"#DEDF21FF", "72"},
                    {"#FFFF00FF", "73"},
                    {"#FFDF00FF", "74"},
                    {"#CEAA00FF", "75"},
                    {"#9C9A00FF", "76"},
                    {"#8C7500FF", "77"},
                    {"#525500FF", "78"},

                    //blue
                    {"#DEBAFFFF", "80"},
                    {"#BD9AEFFF", "81"},
                    {"#6330CEFF", "82"},
                    {"#9C55FFFF", "83"},
                    {"#6300FFFF", "84"},
                    {"#52458CFF", "85"},
                    {"#42009CFF", "86"},
                    {"#210063FF", "87"},
                    {"#211031FF", "88"},

                    //ehm... also blue?
                    {"#BDBAFFFF", "90"},
                    {"#8C9AFFFF", "91"},
                    {"#3130ADFF", "92"},
                    {"#3155EFFF", "93"},
                    {"#0000FFFF", "94"},
                    {"#31308CFF", "95"},
                    {"#0000ADFF", "96"},
                    {"#101063FF", "97"},
                    {"#000021FF", "98"},

                    //green
                    {"#9CEFBDFF", "a0"},
                    {"#63CF73FF", "a1"},
                    {"#216510FF", "a2"},
                    {"#42AA31FF", "a3"},
                    {"#008A31FF", "a4"},
                    {"#527552FF", "a5"},
                    {"#215500FF", "a6"},
                    {"#103021FF", "a7"},
                    {"#002010FF", "a8"},

                    //icky greenish yellow
                    {"#DEFFBDFF", "b0"},
                    {"#CEFF8CFF", "b1"},
                    {"#8CAA52FF", "b2"},
                    {"#ADDF8CFF", "b3"},
                    {"#8CFF00FF", "b4"},
                    {"#ADBA9CFF", "b5"},
                    {"#63BA00FF", "b6"},
                    {"#529A00FF", "b7"},
                    {"#316500FF", "b8"},

                    //Wtf? More blue?
                    {"#BDDFFFFF", "c0"},
                    {"#73CFFFFF", "c1"},
                    {"#31559CFF", "c2"},
                    {"#639AFFFF", "c3"},
                    {"#1075FFFF", "c4"},
                    {"#4275ADFF", "c5"},
                    {"#214573FF", "c6"},
                    {"#002073FF", "c7"},
                    {"#001042FF", "c8"},

                    //gonna call this cyan
                    {"#ADFFFFFF", "d0"},
                    {"#52FFFFFF", "d1"},
                    {"#008ABDFF", "d2"},
                    {"#52BACEFF", "d3"},
                    {"#00CFFFFF", "d4"},
                    {"#429AADFF", "d5"},
                    {"#00658CFF", "d6"},
                    {"#004552FF", "d7"},
                    {"#002031FF", "d8"},

                    //more cyan, because we didn't have enough blue-like colors yet
                    {"#CEFFEFFF", "e0"},
                    {"#ADEFDEFF", "e1"},
                    {"#31CFADFF", "e2"},
                    {"#52EFBDFF", "e3"},
                    {"#00FFCEFF", "e4"},
                    {"#73AAADFF", "e5"},
                    {"#00AA9CFF", "e6"},
                    {"#008A73FF", "e7"},
                    {"#004531FF", "e8"},

                    //also green. Fuck it, whatever.
                    {"#ADFFADFF", "f0"},
                    {"#73FF73FF", "f1"},
                    {"#63DF42FF", "f2"},
                    {"#00FF00FF", "f3"},
                    {"#21DF21FF", "f4"},
                    {"#52BA52FF", "f5"},
                    {"#00BA00FF", "f6"},
                    {"#008A00FF", "f7"},
                    {"#214521FF", "f8"},

                    //greys
                    {"#FFFFFFFF", "0f"},
                    {"#ECECECFF", "1f"},
                    {"#DADADAFF", "2f"},
                    {"#C8C8C8FF", "3f"},
                    {"#B6B6B6FF", "4f"},
                    {"#A3A3A3FF", "5f"},
                    {"#919191FF", "6f"},
                    {"#7F7F7FFF", "7f"},
                    {"#6D6D6DFF", "8f"},
                    {"#5B5B5BFF", "9f"},
                    {"#484848FF", "af"},
                    {"#363636FF", "bf"},
                    {"#242424FF", "cf"},
                    {"#121212FF", "df"},
                    {"#000000FF", "ef"}
                };
                return colorStrings;
            }
        }
    }
}