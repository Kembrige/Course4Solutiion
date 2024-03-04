using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MinVertexCover
{
    public class Point1
    {
        private PointXAML p; //вот это создалось, чтоб в аплоаде ошибку исправить 

        public int number { get; set; }
        public List<int> connections { get; set; }

        public Point1(int number)
        {
            this.number = number;
            connections = new List<int>();
        }

        public Point1(int number, List<int> connections)
        {
            this.connections = connections;
            this.number = number;
        }

        public Point1(PointXAML p)
        {
            this.number = p.Number;
            this.connections = new List<int>(p.Connections);
        }

        private string ConsStr(List<int> cons)
        {
            string str = "";
            foreach (int i in cons) { str += i; str += ","; }
            return str;
        }



   
    public override string ToString()
    {
      return ($"Number: {number} Connected With {ConsStr(connections)}");
    }
    //public Point Clone() => new Point { Number, Connections };
  }
  public static class Count
  {
    static object locker = new object();
    static List<int> Ans = new List<int>();

    static List<Point1> DeleteVertex(List<Point1> tem, int i)
    {
      var ToRet = new List<Point1>(tem.Count);
      foreach (var item in tem)
      {
        var newP = new Point1(item.number, item.connections);
        ToRet.Add(newP);
      }
      for (int k = 0; k < ToRet[i].connections.Count; k++) // проход по коннектам работающей точки графа
      {
        for (int j = 0; j < ToRet.Count; j++)
        {
          if (ToRet[j].number == ToRet[i].connections[k]) // че не нравится блять
          {
            for (int c = 0; c < ToRet[j].connections.Count; c++)
            {
              if (ToRet[j].connections[c] == ToRet[i].number)
              {
                var tet = new List<int>(ToRet[j].connections);
                tet.RemoveAt(c);
                ToRet[j].connections = new List<int>(tet);
                //ToRet[j].Connections.RemoveAt(c);
                //ToRet[i].Connections.Remove(ToRet[j].Number);
                break;
              }
            }
          }
        }
      }
      ToRet[i].connections = new List<int>();

      return ToRet;
    }
    static void Check(List<Point1> tempoints, List<int> tempans)
    {
      bool ch = true;
      foreach (var p in tempoints)
      {
        if (p.connections.Count != 0)
        {
          ch = false;
        }
      }
      if (ch)
      {
        if (Ans.Count > tempans.Count)
        {
          Ans = new List<int>(tempans);
        }
        else if (Ans.Count == 0)
        {
          Ans = new List<int>(tempans);
        }
      }
    }
    static void ShuffleNested(List<Point1> tempoints, int v, List<int> tempans)
    {
      Check(tempoints, tempans);
      int n = v + 1;
      for (int i = n; i < tempoints.Count; i++)
      {
        var newtemp = new List<Point1>(tempoints.Count);
        var newans = new List<int>(tempans);
        newans.Add(tempoints[i].number);
        foreach (var item in tempoints)
        {
          var newP = new Point1((int)item.number, (List<int>)item.connections);
          newtemp.Add(newP);
        }
        newtemp = DeleteVertex(newtemp, i);
        //Console.WriteLine("Workin on " + tempoints[i].Number);
        //foreach (var r in newtemp)
        //{
        //    Console.WriteLine(r);
        //}
        //Console.WriteLine();
        ShuffleNested(newtemp, i, newans);

      }
    }
    static void ShuffleStart(List<Point1> points)
    {
      for (int i = 0; i < points.Count; i++)
      {
        var tempPoints = new List<Point1>(points.Count);
        foreach (var item in points)
        {
          var newP = new Point1((int)item.number, (List<int>)item.connections);
          tempPoints.Add(newP);
        }
        tempPoints = DeleteVertex(tempPoints, i);
        List<int> tempans = new List<int>() { points[i].number };
        ShuffleNested(tempPoints, i, tempans);
      }
    }

    static void ParalCheck(List<Point1> tempoints, List<int> tempans, ref List<int> LocalThreadAns)
    {
      bool ch = true;
      foreach (var p in tempoints)
      {
        if (p.connections.Count != 0)
        {
          ch = false;
        }
      }
      if (ch)
      {
        if (LocalThreadAns.Count > tempans.Count)
        {
          LocalThreadAns = new List<int>(tempans);
        }
        else if (LocalThreadAns.Count == 0)
        {
          LocalThreadAns = new List<int>(tempans);
        }
      }
    }
    static void Parallel_Nested(List<Point1> tempoints, int v, List<int> iterans, ref List<int> localThreadAns)
    {
      ParalCheck(tempoints, iterans, ref localThreadAns);
      int n = v + 1;
      for (int i = n; i < tempoints.Count; i++)
      {
        var newtemp = new List<Point1>(tempoints.Count);
        var newans = new List<int>(iterans);
        newans.Add(tempoints[i].number);
        foreach (var item in tempoints)
        {
          var newP = new Point1((int)item.number, (List<int>)item.connections);
          newtemp.Add(newP);
        }
        newtemp = DeleteVertex(newtemp, i);
        Parallel_Nested(newtemp, i, newans, ref localThreadAns);
      }
    }

    static void Parallel_Shuffle(List<Point1> points)
    {
      Parallel.For(0, points.Count, () => new List<int>(), (i, loop, localans) =>
      {
        var tempoints = new List<Point1>(points.Count);
        foreach (var item in points)
        {
          var newP = new Point1((int)item.number, (List<int>)item.connections);
          tempoints.Add(newP);
        }
        tempoints = DeleteVertex(tempoints, i);
        var iterans = new List<int>() { points[i].number };
        Parallel_Nested(tempoints, i, iterans, ref localans);
        return localans;
      },
      (x) =>
      {
        lock (locker)
        {
          if ((Ans.Count > x.Count) && (x.Count != 0))
          {
            Ans = new List<int>(x);
          }
          else if ((Ans.Count == 0) && (x.Count != 0))
          {
            Ans = new List<int>(x);
          }
          Console.WriteLine();
        }
      }
      );
    }
    public static string Main(List<Point1> points)
    {
      ShuffleStart(points);
      foreach (int i in Ans)
      {
        Console.WriteLine(i);
      }


      Ans = new List<int>();
      Parallel_Shuffle(points);
      Console.WriteLine("\n Parallel Ans:");

      var sb = new StringBuilder();
      foreach (int i in Ans)
      {
        sb.Append($"{i}\n");
      }
      return sb.ToString();
    }
  }
}
