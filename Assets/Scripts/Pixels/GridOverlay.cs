using System;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
   public enum RenderedAxis { X, Y, Z }

   public PixelManager pixelManager;
   public Color color = new Color( 0f, 0f, 0f, 0f );
   public RenderedAxis renderedAxis = RenderedAxis.Z;
   public int renderedAxisCount = 5;
   public Shader shader;

   public Material _lineMaterial;

   void Start()
   {

   }

   void Update()
   {
   }

   void OnRenderObject()
   {
      //CreateLineMaterial();

      var depthCount = pixelManager.depthCount;
      var colCount = pixelManager.colCount;
      var rowCount = pixelManager.rowCount;
      var pixelScale = pixelManager.pixelScale;

      // set the current material
      _lineMaterial.SetPass( 0 );
      //var mat = new Material( new Shader() );
      //mat.SetPass( 0 );
      

      //GL.Color( color );
      DebugHelper.Log( "Grid Color", color.ToString() );

      GL.Begin( GL.LINES );

      //GL.Color( color );
      GL.Color( Color.red );
      //GL.Color( new Color(0,0,0) );

      var startZ = -1 * ( ( ( depthCount * pixelScale ) / 2.0f ) );
      var startX = -1 * ( ( ( colCount * pixelScale ) / 2.0f ) );
      var startY = -1 * ( ( ( rowCount * pixelScale ) / 2.0f ) );

      //Layers
      for ( float j = 0; j <= rowCount; j++ )
      {

         if ( renderedAxis == RenderedAxis.Z )
         {
            //X axis lines
            //for ( float i = 0; i <= depthCount; i++ )
            //{
            GL.Vertex3( startX, startY + j * pixelScale, startZ + ( renderedAxisCount - 1 ) * pixelScale );
            GL.Vertex3( startX + colCount * pixelScale, startY + j * pixelScale, startZ + ( renderedAxisCount - 1 ) * pixelScale );

            GL.Vertex3( startX, startY + j * pixelScale, startZ + renderedAxisCount * pixelScale );
            GL.Vertex3( startX + colCount * pixelScale, startY + j * pixelScale, startZ + renderedAxisCount * pixelScale );
            //}
         }

         //   //Z axis lines
         //   for ( float i = 0; i <= colCount; i++ )
         //   {
         //      GL.Vertex3( startX + i * pixelScale, startY + j * pixelScale, startZ );
         //      GL.Vertex3( startX + i * pixelScale, startY + j * pixelScale, startZ + depthCount * pixelScale );
         //   }
      }

      if ( renderedAxis == RenderedAxis.Z )
      {
         //Y axis lines
         //for ( float i = 0; i <= depthCount; i++ )
         //{
         for ( float k = 0; k <= colCount; k++ )
         {
            GL.Vertex3( startX + k * pixelScale, startY, startZ + ( renderedAxisCount - 1 ) * pixelScale );
            GL.Vertex3( startX + k * pixelScale, startY + rowCount * pixelScale, startZ + ( renderedAxisCount - 1 ) * pixelScale );

            GL.Vertex3( startX + k * pixelScale, startY, startZ + renderedAxisCount * pixelScale );
            GL.Vertex3( startX + k * pixelScale, startY + rowCount * pixelScale, startZ + renderedAxisCount * pixelScale );
         }
         // }
      }

      GL.End();
   }

   void CreateLineMaterial()
   {
      if ( _lineMaterial )
      {
         _lineMaterial.SetColor( "_Color", color );
      }
      // if ( !_lineMaterial )
      //{
      //#pragma warning disable 618
      //         _lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
      //             "SubShader { Pass { " +
      //             "    Blend SrcAlpha OneMinusSrcAlpha " +
      //             "    ZWrite Off Cull Off Fog { Mode Off } " +
      //             "    BindChannels {" +
      //             "      Bind \"vertex\", vertex Bind \"color\", color }" +
      //             "} } }" );
      //#pragma warning restore 618
      // _lineMaterial = new Material( shader );
      // DebugHelper.Log( "Grid Color", _lineMaterial.GetColor( "_Color" ).ToString() );
      //_lineMaterial.SetColor( "_Color", color );
      //_lineMaterial.color = color ;
      //_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
      //_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
      //}
   }
}