using System.Collections.Generic;
using System.Windows.Controls;

namespace MinVertexCover
{
  public class CanvasStruct
  {
    public Canvas Canvas { get; set; }
    public ListOfPoint Points { get; set; }
  }

  public class ListOfPoint : List<PointXAML>
  {

  }

  public class ListOfInt : List<int>
  {

  }

  public class PointXAML
  {
    public int Number { get; set; }
    public int[] Connections { get; set; }
  }

}
