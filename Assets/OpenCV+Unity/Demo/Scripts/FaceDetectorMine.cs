namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using OpenCvSharp;
	using UnityEngine.Tilemaps;

	public class FaceDetectorMine : WebCamera
	{
		public TextAsset faces;
		public TextAsset eyes;
		public TextAsset shapes;

		private FaceProcessorLive<WebCamTexture> processor;

		/*
		tile map stuff
		*/
		public Tilemap tilemap;
		public TileBase tileToPlace;

		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

			byte[] shapeDat = shapes.bytes;

			processor = new FaceProcessorLive<WebCamTexture>();
			processor.Initialize(faces.text, eyes.text, shapes.bytes);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = true;        // enable stabilizer
			processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 20;             // we actually process only each Nth frame (and every frame for skipRate = 0)
		}

		/// <summary>
		/// Per-frame video capture processor
		/// </summary>
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
			// detect everything we're interested in
			processor.ProcessTexture(input, TextureParameters);

			// mark detected objects
			//processor.MarkDetected();

			// processor.Image now holds data we'd like to visualize
			//output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created
			if (processor.Faces.Count != 0) {
				Mat submat = new Mat(processor.Image, processor.Faces[0].Region);
				Cv2.CvtColor(submat, submat, ColorConversionCodes.BGR2GRAY);

				Mat resized = new Mat();
				Cv2.Resize(submat, resized, new Size(100, 100));

				Cv2.Threshold(resized, resized, 100, 255, ThresholdTypes.Binary);

				for (int i = 0; i < resized.Rows; i++) {
					for (int j = 0; j < resized.Cols; j++) {
						double[] pixelValue = resized.GetArray(i, j);
						//Debug.Log($"Pixel value at {i}, {j} is {pixelValue[0]}");
						Vector3Int cellPosition = new Vector3Int(j, 80-i, 0); 
						if (Math.Abs(pixelValue[0]) < 0.00001f) {
							
							tilemap.SetTile(cellPosition, tileToPlace);
						} else {
							tilemap.SetTile(cellPosition, null);
						}
						
					}
				}
				
            	output = Unity.MatToTexture(resized, output);
			}
			

			return true;
		}
	}
}