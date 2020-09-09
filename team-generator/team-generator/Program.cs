using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace team_generator
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random(237580);

            List<Student> students = new List<Student>();

            using (StreamReader sr = new StreamReader("input.csv"))
            {
                // 헤더 읽기
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');

                    Student s = new Student(values[3]);

                    // 수강생의 자발적 팀 정보 읽기
                    int num;
                    if (Int32.TryParse(values[4], out num))
                    {
                        Debug.Assert(num > 0);
                        s.Team = num - 1;
                    }

                    // 수강생의 자발적 4인팀 팀장 여부 읽기
                    if (Int32.TryParse(values[5], out num))
                    {
                        Debug.Assert(num == 0 || num == 1);
                        if (num == 1) s.IsChief = true;
                    }

                    students.Add(s);
                }
            }

            // 팀 생성
            int numOfTeams = students.Count / 4;
            int remainder = students.Count % 4;
            if (remainder == 3) numOfTeams++;
            Team[] teams = new Team[numOfTeams];
            for (int i = 0; i < teams.Length; i++)
            {
                teams[i] = new Team();
                teams[i].Name = IndianName.GetRandomName(rand);
            }

            // 수강생이 4의 배수가 아닌 경우 처리
            if (remainder == 3)
            {
                teams[numOfTeams - 1].maxMembers = 3;
            }
            else
            {
                while (remainder-- > 0)
                {
                    int index = rand.Next(numOfTeams);
                    if (teams[index].maxMembers == 5)
                    {
                        remainder++;
                        continue;
                    }
                    teams[index].maxMembers = 5;
                }
            }

            for (int i = 0; i < students.Count; i++)
            {
                Student s = students[i];
                Team t = null;

                // 자발적 팀 멤버 추가
                if (s.Team >= 0)
                {
                    Debug.Assert(s.Team <= numOfTeams);
                    t = teams[s.Team];
                    t.Add(s);

                    // 자발적 팀장 추가
                    if (s.IsChief)
                    {
                        Debug.Assert(t.Chief == null);
                        t.Chief = s.Name;
                    }
                }
            }

            // 수강생 순서 섞기
            students.Shuffle(rand);

            // 비어 있는 첫 번째 팀 찾기
            int team_idx;
            for (team_idx = 0; team_idx < teams.Length; team_idx++)
            {
                if (teams[team_idx].Count() < teams[team_idx].maxMembers)
                    break;
            }

            // 수강생 팀배정
            for (int student_idx = 0; student_idx < students.Count; student_idx++)
            {
                Student s = students[student_idx];
                Team t = teams[team_idx];

                if (s.Team == -1)
                {
                    t.Add(s);
                    if (t.isFull())
                    {
                        if (++team_idx == numOfTeams) break;
                        t = teams[team_idx];
                        while (t.isFull())
                        {
                            t = teams[++team_idx];
                        }
                    }
                }
            }

            // 팀장 선출
            for (team_idx = 0; team_idx < teams.Length; team_idx++)
            {
                Team t = teams[team_idx];
                Debug.Assert(t.isFull());
                if (t.Chief != null) continue;
                t.Chief = t.Members[rand.Next(t.Count())].Name;
            }

            // 출력
            using (StreamWriter sw = new StreamWriter("output.csv", false, Encoding.UTF8))
            {
                sw.WriteLine("팀번호,팀명,팀장,팀원");
                for (int i = 0; i < teams.Length; i++)
                {
                    Team t = teams[i];

                    string s = "";
                    for (int j = 0; j < t.Count(); j++)
                    {
                        if (t.Members[j].Name == t.Chief) continue;
                        s += (t.Members[j].Name + " ");
                    }

                    sw.WriteLine(String.Format("{0},{1},{2},{3}", i + 1, t.Name, t.Chief, s));
                }
            }
        }
    }

    class Student
    {
        public string Name { get; }
        public int Team { get; set; }
        public bool IsChief { get; set; }

        public Student(string name)
        {
            Name = name;
            Team = -1;
        }
    }

    class Team
    {
        public string Name { get; set; }
        public string Chief { get; set; }
        public int maxMembers { get; set; }

        public List<Student> Members;

        public Team()
        {
            Members = new List<Student>();
            maxMembers = 4;
        }

        public void Add(Student student)
        {
            Debug.Assert(!isFull());
            Members.Add(student);
        }

        public int Count()
        {
            return Members.Count;
        }

        public bool isFull()
        {
            return maxMembers == Members.Count;
        }
    }

    static class PrivateExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random rand)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

}
