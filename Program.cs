using System;

namespace hangle
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("한글 자모 분리 테스트");

            string test = @"안녕! 한글 자모 분리 테스트다.
한글 자모 분리 테스트는 한글 문자열을 읽어서 초.중.종성으로 분리해서... 아~ 왜 이런걸 하고 있지 ㅠㅠ.
   - https://jaecheol-jeong.github.io/doc/
덥다.. ABCD .. ";

            Hangle h = new Hangle();
            string szSplit = h.Split(test);

            Console.WriteLine(szSplit);

            string merge = h.Merge(szSplit);
            Console.WriteLine(merge);
        }
    }

    public class Hangle
    {
        int hanStartPosition = 0xAC00;
        int hanEndPosition = 0xD7AF;

        //한글의 유니코드
        // ㄱ ㄲ ㄴ ㄷ ㄸ ㄹ ㅁ ㅂ ㅃ ㅅ ㅆ ㅇ ㅈ ㅉ ㅊ ㅋ ㅌ ㅍ ㅎ
        char[] ChoSung ={ 'ㄱ','ㄲ', 'ㄴ', 'ㄷ','ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ' ,'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ','ㅌ','ㅍ','ㅎ' };

        // ㅏ ㅐ ㅑ ㅒ ㅓ ㅔ ㅕ ㅖ ㅗ ㅘ ㅙ ㅚ ㅛ ㅜ ㅝ ㅞ ㅟ ㅠ ㅡ ㅢ ㅣ
        char[] JungSung = { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ' , 'ㅘ' , 'ㅙ', 'ㅚ' 
                            , 'ㅛ' , 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ' , 'ㅢ', 'ㅣ' };

        // ㄱ ㄲ ㄳ ㄴ ㄵ ㄶ ㄷ ㄹ ㄺ ㄻ ㄼ ㄽ ㄾ ㄿ ㅀ ㅁ ㅂ ㅄ ㅅ ㅆ ㅇ ㅈ ㅊ ㅋ ㅌ ㅍ ㅎ
        char[] JongSung = { ' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ',
                                'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
                                

        public string Split(string str)
        {
            int a, b, c, u, d;
            string result = string.Empty;

            for(int i = 0; i < str.Length; i++)
            {
                int x = (int)str[i];
                if(x >= hanStartPosition && x <= hanEndPosition) //한글
                {
                    u = x - 0xAC00;
                    a = u / (21 * 28);
                    c = u % (21 * 28);
                    b = c / 28;
                    d = c % 28;
                    
                    //Console.WriteLine($"{u} {a} {b} {c} {d}");
                    result += string.Format("{0}{1}", ChoSung[a], JungSung[b]);

                    // $c가 0이면, 즉 받침이 있을경우
                    if (c != 0 && d > 0)
                        result += string.Format("{0}", JongSung[d]);
                    
                }
                else
                {
                    result += $"{(char)x}";
                }
            }        
            
            //Console.WriteLine(result);
            return result;
        }

        public string Merge(string str)
        {
            string[] words = str.Split(' ');

            string result = string.Empty;
            foreach(string w in words)
            {
                result += MergeMake(w) + " ";
            }

            return result;
        }

        public string MergeMake(string str)
        {
            //“각” = 0xAC00+(초*21+중)*28+종 = 0xAC01
            string result = string.Empty;
            int cho = 0, jung = 0, jong = 0;

            for(int i = 0; i < str.Length; )
            {
                if(i >= str.Length) break;

                if(IsHangle(str[i]))
                {
                    char ch = (i < str.Length) ? str[i] : ' ';
                    char nxt = ((i+1) < str.Length) ? str[i+1] : ' ';
                    
                    if(IsChosung(ch, nxt))
                    {
                        cho = Array.IndexOf(ChoSung, ch);

                        if(IsJungsung(nxt) && IsHangle(nxt))
                        {
                            jung = Array.IndexOf(JungSung, nxt);
                        }
                        else
                        {
                            result += $"{(char)nxt}";
                            i++;
                            continue;
                        }
                        char nnxt1 = ((i+2) < str.Length) ? str[i+2] : (char)0xD7B0;
                        char nnxt2 = ((i+3) < str.Length) ? str[i+3] : ' ';

                        
                        if(IsJongsung(nnxt1, nnxt2))
                        {
                            jong = Array.IndexOf(JongSung, nnxt1);
                            i += 3;
                        }
                        else
                        {
                            jong = 0;
                            i += 2;
                        }                        

                        int n = hanStartPosition + (cho * 21 + jung) * 28 + jong;
                        result += Convert.ToChar(n).ToString();
                    }
                    else
                    {
                        result += $"{str[i]}";
                        i++;
                    }
                }
                else
                {
                    result += $"{str[i]}";
                    i++;
                }
            }

            return result;           
        }

        private bool IsHangle(char ch)
        {
            if(Array.IndexOf(JongSung, ch) > -1)
            {
                return true;
            }
            if(Array.IndexOf(JungSung, ch) > -1)
            {
                return true;
            }
            if(Array.IndexOf(ChoSung, ch) > -1)
            {
                return true;
            }
            return false;
        }

        private bool IsAlpabet(char ch)
        {
            int x = (int)ch;

            if(x <= 0x024f) return true;

            return false;
        }

        private bool IsChosung(char ch1, char ch2)
        {
            if(Array.IndexOf(ChoSung, ch1) > -1)
            {
                if(Array.IndexOf(JungSung, ch2) > -1)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private bool IsJungsung(char ch1)
        {
            if(Array.IndexOf(JungSung, ch1) > -1)
            {
                return true;
            }
            return false;
        }

        private bool IsJongsung(char ch1, char ch2)
        {
            if(Array.IndexOf(JongSung, ch1) > -1)
            {
                if(Array.IndexOf(ChoSung, ch2) > -1 || ch2 == ' ' || IsAlpabet(ch2))
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private (bool, bool) CheckAB(char ch1, char ch2)
        {
            return (true, false);
        }
    }
}
