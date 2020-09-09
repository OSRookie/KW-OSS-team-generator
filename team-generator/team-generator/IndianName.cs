using System;
using System.Collections.Generic;
using System.Text;

namespace team_generator
{
    class IndianName
    {
        static string[] First = { "말 많은", "푸른", "어두운", "조용한", "웅크린", "백색", "지혜로운", "용감한", "날카로운", "욕심 많은" };
        static string[] Middle = { "늑대", "태양", "양", "매", "황소", "불꽃", "나무", "달빛", "말", "돼지", "하늘", "바람" };
        static string[] Last = {"", "와/과 함께 춤을", "의 기상", "는/은 그림자 속에", "의 환생", "의 죽음", " 아래에서", "를/을 보라",
                                "가/이 노래하다", "의 그림자", "의 일격", "에게 쫓기는 자들", "의 행진", "의 왕", "의 유령들", "를/을 죽인 자들",
                                "는/은 매일 잔다", "처럼", "의 고향", "의 전사", "는/은 우리들의 친구", "의 노래", "의 정령", "의 파수꾼",
                                "의 악마들", "와 같은 사람들", "를/을 쓰러뜨린 자들", "의 혼", "는/은 말이 없다" };

        public static string GetRandomName(Random rand)
        {
            string name = First[rand.Next(First.Length)];
            name += (" " + Middle[rand.Next(Middle.Length)]);

            // 받침 처리
            string last = Last[rand.Next(Last.Length)];
            if (last.Contains('/'))
            {
                char lastChar = name[name.Length - 1];
                if ((lastChar - 0xAC00) % 28 == 0) last = last[0] + last.Substring(3);
                else last = last.Substring(2);
            }
            name += last;

            return name;
        }
    }
}
