using System;
using System.Collections;
using System.Collections.Generic;
public class Edge {
  public int from;
  public int to;

  public Edge(int from = -1, int to = -1){
    this.from = from;
    this.to = to;
  }

  public int GetFrom(){
    return this.from;
  }

  public int GetTo(){
    return this.to;
  }
}
public class DistOriginal
{
  public double distance; 
  public int parentVert;
  public DistOriginal(int pv, double d)
  {
    distance = d; parentVert = pv;
  }
}
public class Vertex
{
  public string label; public bool isInTree; public double x; public double y;
  public Vertex(string lab, double x, double y) 
  { 
    label = lab; isInTree = false; 
    this.x = x; this.y = y;
  }
}
public class Graph
{
  private const int max_verts = 20;
  public int infinity = 1000000; Vertex[] vertexList; public double[,] adjMat;
  int nVerts; int nTree; DistOriginal[] sPath;
  int currentVert; double startToCurrent;
  List<Edge> edges = new List<Edge>();
  public Graph() 
  {
    vertexList = new Vertex[max_verts];
    adjMat = new double[max_verts, max_verts];
    nVerts = 0; nTree = 0;
    for (int j = 0; j <= max_verts - 1; j++)
      for (int k = 0; k <= max_verts - 1; k++)
        adjMat[j, k] = infinity;
    sPath = new DistOriginal[max_verts];

    //
    string[] VertexData = File.ReadAllLines(@"VertexList.txt");
    foreach (var line in VertexData){
      string[] s = line.Split(" ");
      AddVertex(s[0], int.Parse(s[1]), int.Parse(s[2]));
    }

    // AddVertex("0hcm",3,4); AddVertex("1vinh",4,3);
    // AddVertex("2danang",5,1); AddVertex("3camau",4,1);
    // AddVertex("4halong",9,0); AddVertex("5hanoi",7,3);
    // AddVertex("6binhdinh",6,4); 

    // doc file de addEdge
    string[] EdgeData = File.ReadAllLines(@"EdgeList.txt");
    foreach (var line in EdgeData){
      string[] s = line.Split(" ");
      AddEdge(int.Parse(s[0]),int.Parse(s[1]));
    }
    }
  public void AddVertex(string lab, double x, double y)
  {
    vertexList[nVerts] = new Vertex(lab, x, y); nVerts++;
  }
  public void AddEdge(int start, int theEnd)
  {
    adjMat[start, theEnd] = Math.Round(Math.Sqrt(Math.Pow((vertexList[start].x - vertexList[theEnd].x),2) + Math.Pow((vertexList[start].y - vertexList[theEnd].y),2)),2) ;
    adjMat[theEnd, start] = adjMat[start, theEnd];
  }
  public void Path(int from, int to)
  {
    int startTree = from;
    vertexList[startTree].isInTree = true;
    nTree = 0;
    for (int j = 0; j <= nVerts; j++)
    {
      double tempDist = adjMat[startTree, j];
      sPath[j] = new DistOriginal(startTree, tempDist);
    }
    while (nTree < nVerts) 
    {
      if(nTree == from) nTree++;
      int indexMin = GetMin();
      double minDist = sPath[indexMin].distance;
      currentVert = indexMin;
      startToCurrent = minDist;
      vertexList[currentVert].isInTree = true;
      nTree++;
      AdjustShortPath(from, to);
      if(indexMin == to) break;
    }
    // DisplayPaths();
    FindShortestPath(from, to);
    nTree = 0;
    for (int j = 0; j <= nVerts - 1; j++)
      vertexList[j].isInTree = false;
  }
  // Get index of vertex has shortest distance
  public int GetMin()
  {
    double minDist = infinity;
    int indexMin = 0;
    for (int j = 1; j <= nVerts - 1; j++)
      if (!(vertexList[j].isInTree) && sPath[j].distance < minDist)
      {
        minDist = sPath[j].distance; indexMin = j;
      }
    return indexMin;
  }
  // calculate path of adjust vertex
  public void AdjustShortPath(int from, int to)
  {
    int column = 0;
    while (column < nVerts)
      if (vertexList[column].isInTree || column == from) column++;

      else
      {
        double currentToFring = adjMat[currentVert, column];
        double startToFringe = startToCurrent + currentToFring;
        double sPathDist = sPath[column].distance;
        if (startToFringe < sPathDist)
        {
          sPath[column].parentVert = currentVert;
          sPath[column].distance = startToFringe;
        }
        column++;
      }
  }
  public void DisplayPaths()
  {
    for (int j = 0; j <= nVerts - 1; j++)
    {
      Console.Write(vertexList[j].label + "=");
      if (sPath[j].distance == infinity) Console.Write("inf");
      else Console.Write(sPath[j].distance);
      string parent = vertexList[sPath[j].parentVert].label;
      Console.Write(sPath[j].parentVert);
      Console.Write("(" + parent + ") ");
     
    }
  }
  public void FindShortestPath(int from, int to) {
    int cur = to;
    for (int j = 0; j < nVerts ; j++)
    {
      string parent = vertexList[sPath[j].parentVert].label;
      Edge edge = new Edge(j, sPath[j].parentVert);
      edges.Add(edge);
    }

    Console.WriteLine("From: " + from + " To: " + to);
    string result = "" + vertexList[to].label;
    while (cur != from)
      for(int i = 0 ; i < edges.Count() ; i++){

        if(edges.ElementAt(i).GetFrom() == cur){
          cur = edges.ElementAt(i).GetTo();
          result = vertexList[cur].label + " -> " + result;
          break;

        }
      }
    Console.WriteLine(result);
    Console.WriteLine(">>Distance: {0:N2}km", startToCurrent);
  }

  public void DisplayMatrix(){
    for (int i = 0 ; i < adjMat.GetLength(0) ; i++){
      for (int j = 0 ; j < adjMat.GetLength(1) ; j++){
        if (adjMat[i,j] == infinity) Console.Write("{0,5}","inf");
        else Console.Write("{0,5}",adjMat[i,j]);
      }
    Console.WriteLine();
    }
  }
}

public class Program
{
  public static void Main()
  {
    Console.Clear();
    Graph theGraph = new Graph();
    
    

    Console.WriteLine("Shortest paths:"); theGraph.Path(0,1);
    theGraph.DisplayMatrix();
    // Console.ReadLine();
  }
}