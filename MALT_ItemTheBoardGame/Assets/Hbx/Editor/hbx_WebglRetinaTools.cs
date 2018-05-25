//
// Written by Thomas Hogarth, Hogbox Studios Ltd
// Developed against WebGL build from Unity 5.4.1f1
//
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace hbx {

	public class WebGLRetinaTools
	{
		const string ProgressTitle = "Applying Retina Fix";
		const string RelFolder = "Release";
		const string DevFolder = "Development";
		const string JsExt = ".js";
		const string JsgzExt = ".jsgz";
		static readonly string[] SourceFileTypes = {JsExt, JsgzExt};
		static readonly string[] ExcludeFileNames = {"UnityLoader"};
		
		[MenuItem("Hbx/WebGL Tools/Retina Fix Last Build", false, 0)]
		public static void RetinaFixLastBuild()
		{
			if(EditorUserBuildSettings.development) {
				RetinaFixDevelopmentBuild();
			} else {
				RetinaFixReleaseBuild();
			}
		}

		[MenuItem("Hbx/WebGL Tools/Retina Fix Existing Build", false, 1)]
		public static void RetinaFixExistingBuild()
		{
			string path = EditorUtility.OpenFolderPanel("Select a WebGL build folder", "", "" );
			if(string.IsNullOrEmpty(path)) {
				UnityEngine.Debug.LogWarning("WebGLRetinaTools: No build folder selected.");
				return;
			}

			// look for release and/or development folders
			if(Directory.Exists(Path.Combine(path, RelFolder))) {
				RetinaFixReleaseBuild(path);
			}
			if(Directory.Exists(Path.Combine(path, DevFolder))) {
				RetinaFixDevelopmentBuild(path);
			}
		}

		//
		// Opens the jsgz and/or the js file in the current webgl build folder 
		// and inserts devicePixelRatio accordingly to add support for retina/hdpi 
		//
		//[MenuItem("Hbx/WebGL Tools/Retina Fix Release Build", false, 11)]
		public static void RetinaFixReleaseBuild(string buildOverridePath = "")
		{
			UnityEngine.Debug.Log("WebGLRetinaTools: Fix release build started.");
			
			// get path of the last webgl build or use over ride path
			string webglBuildPath = string.IsNullOrEmpty(buildOverridePath) ? EditorUserBuildSettings.GetBuildLocation(BuildTarget.WebGL) : buildOverridePath;
			string releaseFolderPath = Path.Combine(webglBuildPath, RelFolder);

			if(string.IsNullOrEmpty(releaseFolderPath)) {
				UnityEngine.Debug.LogError("WebGLRetinaTools: WebGL build path is empty, have you created a release WebGL build yet?");
				return;
			}
	
			// check there is a release folder
			if(!Directory.Exists(releaseFolderPath)) {
				UnityEngine.Debug.LogError("WebGLRetinaTools: Couldn't find Release folder for WebGL build at path:\n" + releaseFolderPath);
				return;
			}

			// find source files in release folder and fix
			string[] sourceFiles = FindSourceFilesInBuildFolder(releaseFolderPath);
			foreach(string sourceFile in sourceFiles)
				FixSourceFile(sourceFile, true);
	
			UnityEngine.Debug.Log("WebGLRetinaTools: Fixed " + sourceFiles.Length + " release source files.");

			EditorUtility.ClearProgressBar();
		}

		//
		// Opens the jsgz and/or the js file in the current webgl build folder 
		// and inserts devicePixelRatio accordingly to add support for retina/hdpi 
		//
		//[MenuItem("Hbx/WebGL Tools/Retina Fix Development Build", false, 12)]
		public static void RetinaFixDevelopmentBuild(string buildOverridePath = "")
		{
			UnityEngine.Debug.Log("WebGLRetinaTools: Fix development build started.");
			
			// get path of the last webgl build or use over ride path
			string webglBuildPath = string.IsNullOrEmpty(buildOverridePath) ? EditorUserBuildSettings.GetBuildLocation(BuildTarget.WebGL) : buildOverridePath;
			string developmentFolderPath = Path.Combine(webglBuildPath, DevFolder);	

			if(string.IsNullOrEmpty(developmentFolderPath)) {
				UnityEngine.Debug.LogError("WebGLRetinaTools: WebGL build path is empty, have you created a development WebGL build yet?");
				return;
			}
	
			// check there is a development folder
			if(!Directory.Exists(developmentFolderPath)) {
				UnityEngine.Debug.LogError("WebGLRetinaTools: Couldn't find development folder for WebGL build at path:\n" + developmentFolderPath);
				return;
			}

			// find source files in development folder and fix
			string[] sourceFiles = FindSourceFilesInBuildFolder(developmentFolderPath);
			foreach(string sourceFile in sourceFiles)
				FixSourceFile(sourceFile, false);

			UnityEngine.Debug.Log("WebGLRetinaTools: Fixed " + sourceFiles.Length + " development source files.");
	
			EditorUtility.ClearProgressBar();
		}

		//
		// Fix a source file based on it's extension type
		//
		static void FixSourceFile(string aSourceFile, bool isRelease) {
			UnityEngine.Debug.Log("WebGLRetinaTools: Fixing " + aSourceFile);
			string ext = Path.GetExtension(aSourceFile);
			if(ext == JsExt) {
				FixJSFile(aSourceFile, isRelease);
			} else if(ext == JsgzExt) {
				FixJSGZFile(aSourceFile, isRelease);
			}
		}
		
		//
		// Fix a standard .js file
		//
		static void FixJSFile(string jsPath, bool isRelease) {

			EditorUtility.DisplayProgressBar(ProgressTitle, "Opening js...", 0.0f);
	
			// load the uncompressed js code (this might trip over on large projects)
			StringBuilder source = new StringBuilder(File.ReadAllText(jsPath));
	
			EditorUtility.DisplayProgressBar(ProgressTitle, "Fixing js source...", 0.5f);
	
			if(isRelease) {
				FixJSFileContentsRelease(ref source);
			} else {
				FixJSFileContentsDevelopment(ref source);
			}
	
			EditorUtility.DisplayProgressBar(ProgressTitle, "Saving js...", 1.0f);
	
			// save the file
			File.WriteAllText(jsPath, source.ToString());
		}
	
		//
		// Fix a compressed jsgz file, decompresses and recompress accordingly
		//
		static void FixJSGZFile(string jsgzPath, bool isRelease) {
	
			EditorUtility.DisplayProgressBar(ProgressTitle, "Uncompressing jsgz...", 0.0f);
	
			string buildJSFolder = Path.GetDirectoryName(jsgzPath);
			string buildJSName = Path.GetFileNameWithoutExtension(jsgzPath);

			// create buffer for decompressing jsgz
			const int size = 134217728; //128MB should be more than enough :/
			byte[] buffer = new byte[size];
			int readcount = 0;

			// open jsgz file
			using (FileStream inputFileStream = new FileStream(jsgzPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// create demcompress stream from opened jsgz file
            	using (GZipStream decompressionStream = new GZipStream(inputFileStream, CompressionMode.Decompress))
				{
					// read decompressed buffer
					readcount = decompressionStream.Read(buffer, 0, size);
					inputFileStream.Close();
		
					if (readcount <= 0) {
						UnityEngine.Debug.LogError("WebGLRetinaTools: Failed to read from jsgz file can't continue.");
						return;
					}
				}
			}
	
			// create a string builder to edit from the decompressed buffer
			string decompressedSourceStr = System.Text.Encoding.UTF8.GetString(buffer, 0, readcount);
			StringBuilder source = new StringBuilder(decompressedSourceStr);
	
			EditorUtility.DisplayProgressBar(ProgressTitle, "Fixing jsgz source...", 0.5f);
	
			// fix the source
			if(isRelease) {
				FixJSFileContentsRelease(ref source);
			} else {
				FixJSFileContentsDevelopment(ref source);
			}
	
			EditorUtility.DisplayProgressBar(ProgressTitle, "Recompressing jsgz...", 1.0f);
	
			// write the edited text to file
			string editedSourcePath = Path.Combine(buildJSFolder, buildJSName + JsExt);
			File.WriteAllText(editedSourcePath, source.ToString(), Encoding.UTF8);

			// compress the edited file
	        using (FileStream editedSourceStream = File.OpenRead(editedSourcePath))
 			{
	        	using (FileStream compressOutputStream = File.Create(jsgzPath))
				{
	
	        		buffer = new byte[editedSourceStream.Length];
	        		editedSourceStream.Read(buffer, 0, buffer.Length);
	
	        		using (GZipStream output = new GZipStream(compressOutputStream, CompressionMode.Compress))
	        		{
	            		output.Write(buffer, 0, buffer.Length);
	        		}

					compressOutputStream.Close();
				}
				editedSourceStream.Close();
			}
			// delete the temp edited file
			File.Delete(editedSourcePath);			
		}
		
		//
		// Search folder path for all supported SourceFileTypes
		// excluding any with names matching ExcludeFileNames
		//
		static string[] FindSourceFilesInBuildFolder(string aBuildPath) {
			string[] files = Directory.GetFiles(aBuildPath);
			List<string> found = new List<string>();
			foreach(string file in files) {
				string ext = Path.GetExtension(file); 
				if(Array.IndexOf(SourceFileTypes, ext) == -1) continue;
				string name = Path.GetFileNameWithoutExtension(file);
				if(Array.IndexOf(ExcludeFileNames, name) != -1) continue;
				found.Add(file);
			}
			return found.ToArray();
		}
	
		//
		// Perform the find and replace hack for a release source
		//
		static void FixJSFileContentsRelease(ref StringBuilder source) {
			// fix fillMouseEventData
			source.Replace("fillMouseEventData:(function(eventStruct,e,target){HEAPF64[eventStruct>>3]=JSEvents.tick();HEAP32[eventStruct+8>>2]=e.screenX;HEAP32[eventStruct+12>>2]=e.screenY;HEAP32[eventStruct+16>>2]=e.clientX;HEAP32[eventStruct+20>>2]=e.clientY;HEAP32[eventStruct+24>>2]=e.ctrlKey;HEAP32[eventStruct+28>>2]=e.shiftKey;HEAP32[eventStruct+32>>2]=e.altKey;HEAP32[eventStruct+36>>2]=e.metaKey;HEAP16[eventStruct+40>>1]=e.button;HEAP16[eventStruct+42>>1]=e.buttons;HEAP32[eventStruct+44>>2]=e[\"movementX\"]||e[\"mozMovementX\"]||e[\"webkitMovementX\"]||e.screenX-JSEvents.previousScreenX;HEAP32[eventStruct+48>>2]=e[\"movementY\"]||e[\"mozMovementY\"]||e[\"webkitMovementY\"]||e.screenY-JSEvents.previousScreenY;if(Module[\"canvas\"]){var rect=Module[\"canvas\"].getBoundingClientRect();HEAP32[eventStruct+60>>2]=e.clientX-rect.left;HEAP32[eventStruct+64>>2]=e.clientY-rect.top}else{HEAP32[eventStruct+60>>2]=0;HEAP32[eventStruct+64>>2]=0}if(target){var rect=JSEvents.getBoundingClientRectOrZeros(target);HEAP32[eventStruct+52>>2]=e.clientX-rect.left;HEAP32[eventStruct+56>>2]=e.clientY-rect.top}else{HEAP32[eventStruct+52>>2]=0;HEAP32[eventStruct+56>>2]=0}JSEvents.previousScreenX=e.screenX;JSEvents.previousScreenY=e.screenY})",
				           "fillMouseEventData:(function(eventStruct,e,target){var devicePixelRatio = window.devicePixelRatio || 1;HEAPF64[eventStruct>>3]=JSEvents.tick();HEAP32[eventStruct+8>>2]=e.screenX*devicePixelRatio;HEAP32[eventStruct+12>>2]=e.screenY*devicePixelRatio;HEAP32[eventStruct+16>>2]=e.clientX*devicePixelRatio;HEAP32[eventStruct+20>>2]=e.clientY*devicePixelRatio;HEAP32[eventStruct+24>>2]=e.ctrlKey;HEAP32[eventStruct+28>>2]=e.shiftKey;HEAP32[eventStruct+32>>2]=e.altKey;HEAP32[eventStruct+36>>2]=e.metaKey;HEAP16[eventStruct+40>>1]=e.button;HEAP16[eventStruct+42>>1]=e.buttons;HEAP32[eventStruct+44>>2]=e[\"movementX\"]||e[\"mozMovementX\"]||e[\"webkitMovementX\"]||(e.screenX*devicePixelRatio)-JSEvents.previousScreenX;HEAP32[eventStruct+48>>2]=e[\"movementY\"]||e[\"mozMovementY\"]||e[\"webkitMovementY\"]||(e.screenY*devicePixelRatio)-JSEvents.previousScreenY;if(Module[\"canvas\"]){var rect=Module[\"canvas\"].getBoundingClientRect();HEAP32[eventStruct+60>>2]=(e.clientX-rect.left)*devicePixelRatio;HEAP32[eventStruct+64>>2]=(e.clientY-rect.top)*devicePixelRatio}else{HEAP32[eventStruct+60>>2]=0;HEAP32[eventStruct+64>>2]=0}if(target){var rect=JSEvents.getBoundingClientRectOrZeros(target);HEAP32[eventStruct+52>>2]=(e.clientX-rect.left)*devicePixelRatio;HEAP32[eventStruct+56>>2]=(e.clientY-rect.top)*devicePixelRatio;}else{HEAP32[eventStruct+52>>2]=0;HEAP32[eventStruct+56>>2]=0}JSEvents.previousScreenX=e.screenX*devicePixelRatio;JSEvents.previousScreenY=e.screenY*devicePixelRatio})");
	
			// fix SystemInfo screen width height 
			source.Replace("var systemInfo={get:(function(){if(systemInfo.hasOwnProperty(\"hasWebGL\"))return this;var unknown=\"-\";this.width=screen.width?screen.width:0;this.height=screen.height?screen.height:0;",
				           "var systemInfo={get:(function(){if(systemInfo.hasOwnProperty(\"hasWebGL\"))return this;var unknown=\"-\";var devicePixelRatio = window.devicePixelRatio || 1;this.width=screen.width?screen.width*devicePixelRatio:0;this.height=screen.height?screen.height*devicePixelRatio:0;");
	
			// fix _JS_SystemInfo_GetCurrentCanvasHeight
			source.Replace("function _JS_SystemInfo_GetCurrentCanvasHeight(){return Module[\"canvas\"].clientHeight}",
				           "function _JS_SystemInfo_GetCurrentCanvasHeight(){var devicePixelRatio = window.devicePixelRatio || 1;return Module[\"canvas\"].clientHeight*devicePixelRatio;}");
	
			// fix get _JS_SystemInfo_GetCurrentCanvasWidth
			source.Replace("function _JS_SystemInfo_GetCurrentCanvasWidth(){return Module[\"canvas\"].clientWidth}",
				           "function _JS_SystemInfo_GetCurrentCanvasWidth(){var devicePixelRatio = window.devicePixelRatio || 1;return Module[\"canvas\"].clientWidth*devicePixelRatio;}");

			// fix updateCanvasDimensions (it removes the canvas style width height which prevents the fullscreen toggle via style)
			source.Replace("else{if(canvas.width!=wNative)canvas.width=wNative;if(canvas.height!=hNative)canvas.height=hNative;if(typeof canvas.style!=\"undefined\"){if(w!=wNative||h!=hNative){canvas.style.setProperty(\"width\",w+\"px\",\"important\");canvas.style.setProperty(\"height\",h+\"px\",\"important\")}else{canvas.style.removeProperty(\"width\");canvas.style.removeProperty(\"height\")}}}",
						   "else{if(canvas.width!=wNative)canvas.width=wNative;if(canvas.height!=hNative)canvas.height=hNative;if(typeof canvas.style!=\"undefined\"){if(w!=wNative||h!=hNative){canvas.style.setProperty(\"width\",w+\"px\",\"important\");canvas.style.setProperty(\"height\",h+\"px\",\"important\")}else{}}}");
		}

		//
		// Perform the find and replace hack for a development source
		//
		static void FixJSFileContentsDevelopment(ref StringBuilder source) {

			// fix fill mouse event
			string findFillMouseString = 
@" fillMouseEventData: (function(eventStruct, e, target) {
  HEAPF64[eventStruct >> 3] = JSEvents.tick();
  HEAP32[eventStruct + 8 >> 2] = e.screenX;
  HEAP32[eventStruct + 12 >> 2] = e.screenY;
  HEAP32[eventStruct + 16 >> 2] = e.clientX;
  HEAP32[eventStruct + 20 >> 2] = e.clientY;
  HEAP32[eventStruct + 24 >> 2] = e.ctrlKey;
  HEAP32[eventStruct + 28 >> 2] = e.shiftKey;
  HEAP32[eventStruct + 32 >> 2] = e.altKey;
  HEAP32[eventStruct + 36 >> 2] = e.metaKey;
  HEAP16[eventStruct + 40 >> 1] = e.button;
  HEAP16[eventStruct + 42 >> 1] = e.buttons;
  HEAP32[eventStruct + 44 >> 2] = e[""movementX""] || e[""mozMovementX""] || e[""webkitMovementX""] || e.screenX - JSEvents.previousScreenX;
  HEAP32[eventStruct + 48 >> 2] = e[""movementY""] || e[""mozMovementY""] || e[""webkitMovementY""] || e.screenY - JSEvents.previousScreenY;
  if (Module[""canvas""]) {
   var rect = Module[""canvas""].getBoundingClientRect();
   HEAP32[eventStruct + 60 >> 2] = e.clientX - rect.left;
   HEAP32[eventStruct + 64 >> 2] = e.clientY - rect.top;
  } else {
   HEAP32[eventStruct + 60 >> 2] = 0;
   HEAP32[eventStruct + 64 >> 2] = 0;
  }
  if (target) {
   var rect = JSEvents.getBoundingClientRectOrZeros(target);
   HEAP32[eventStruct + 52 >> 2] = e.clientX - rect.left;
   HEAP32[eventStruct + 56 >> 2] = e.clientY - rect.top;
  } else {
   HEAP32[eventStruct + 52 >> 2] = 0;
   HEAP32[eventStruct + 56 >> 2] = 0;
  }
  JSEvents.previousScreenX = e.screenX;
  JSEvents.previousScreenY = e.screenY;
 }),";

			string replaceFillMouseString = 
@" fillMouseEventData: (function(eventStruct, e, target) {
  var devicePixelRatio = window.devicePixelRatio || 1;
  HEAPF64[eventStruct >> 3] = JSEvents.tick();
  HEAP32[eventStruct + 8 >> 2] = e.screenX*devicePixelRatio;
  HEAP32[eventStruct + 12 >> 2] = e.screenY*devicePixelRatio;
  HEAP32[eventStruct + 16 >> 2] = e.clientX*devicePixelRatio;
  HEAP32[eventStruct + 20 >> 2] = e.clientY*devicePixelRatio;
  HEAP32[eventStruct + 24 >> 2] = e.ctrlKey;
  HEAP32[eventStruct + 28 >> 2] = e.shiftKey;
  HEAP32[eventStruct + 32 >> 2] = e.altKey;
  HEAP32[eventStruct + 36 >> 2] = e.metaKey;
  HEAP16[eventStruct + 40 >> 1] = e.button;
  HEAP16[eventStruct + 42 >> 1] = e.buttons;
  HEAP32[eventStruct + 44 >> 2] = e[""movementX""] || e[""mozMovementX""] || e[""webkitMovementX""] || (e.screenX*devicePixelRatio) - JSEvents.previousScreenX;
  HEAP32[eventStruct + 48 >> 2] = e[""movementY""] || e[""mozMovementY""] || e[""webkitMovementY""] || (e.screenY*devicePixelRatio) - JSEvents.previousScreenY;
  if (Module[""canvas""]) {
   var rect = Module[""canvas""].getBoundingClientRect();
   HEAP32[eventStruct + 60 >> 2] = (e.clientX - rect.left) * devicePixelRatio;
   HEAP32[eventStruct + 64 >> 2] = (e.clientY - rect.top) * devicePixelRatio;
  } else {
   HEAP32[eventStruct + 60 >> 2] = 0;
   HEAP32[eventStruct + 64 >> 2] = 0;
  }
  if (target) {
   var rect = JSEvents.getBoundingClientRectOrZeros(target);
   HEAP32[eventStruct + 52 >> 2] = (e.clientX - rect.left) * devicePixelRatio;
   HEAP32[eventStruct + 56 >> 2] = (e.clientY - rect.top) * devicePixelRatio;
  } else {
   HEAP32[eventStruct + 52 >> 2] = 0;
   HEAP32[eventStruct + 56 >> 2] = 0;
  }
  JSEvents.previousScreenX = e.screenX*devicePixelRatio;
  JSEvents.previousScreenY = e.screenY*devicePixelRatio;
 }),";

			source.Replace(findFillMouseString, replaceFillMouseString);
	

			// fix SystemInfo screen width height 
			string findSystemInfoString = 
@"var systemInfo = {
 get: (function() {
  if (systemInfo.hasOwnProperty(""hasWebGL"")) return this;
  var unknown = ""-"";
  this.width = screen.width ? screen.width : 0;
  this.height = screen.height ? screen.height : 0;";

			string replaceSystemInfoString = 
@"var systemInfo = {
 get: (function() {
  if (systemInfo.hasOwnProperty(""hasWebGL"")) return this;
  var unknown = ""-"";
  var devicePixelRatio = window.devicePixelRatio || 1;
  this.width = screen.width ? screen.width*devicePixelRatio : 0;
  this.height = screen.height ? screen.height*devicePixelRatio : 0;";
			

			source.Replace(findSystemInfoString, replaceSystemInfoString);
	

			// fix _JS_SystemInfo_GetCurrentCanvasHeight
			
			string findGetCurrentCanvasHeightString =
@"function _JS_SystemInfo_GetCurrentCanvasHeight() {
 return Module[""canvas""].clientHeight;
}";

			string replaceGetCurrentCanvasHeightString =
@"function _JS_SystemInfo_GetCurrentCanvasHeight() {
 var devicePixelRatio = window.devicePixelRatio || 1;
 return Module[""canvas""].clientHeight*devicePixelRatio;
}";

			source.Replace(findGetCurrentCanvasHeightString, replaceGetCurrentCanvasHeightString);
	

			// fix get _JS_SystemInfo_GetCurrentCanvasWidth

			string findGetCurrentCanvasWidthString =
@"function _JS_SystemInfo_GetCurrentCanvasWidth() {
 return Module[""canvas""].clientWidth;
}";

			string replaceGetCurrentCanvasWidthString =
@"function _JS_SystemInfo_GetCurrentCanvasWidth() {
 var devicePixelRatio = window.devicePixelRatio || 1;
 return Module[""canvas""].clientWidth*devicePixelRatio;
}";

			source.Replace(findGetCurrentCanvasWidthString, replaceGetCurrentCanvasWidthString);

			
			// fix updateCanvasDimensions

			string findUpdateCanvasString =
@"else {
   if (canvas.width != wNative) canvas.width = wNative;
   if (canvas.height != hNative) canvas.height = hNative;
   if (typeof canvas.style != ""undefined"") {
    if (w != wNative || h != hNative) {
     canvas.style.setProperty(""width"", w + ""px"", ""important"");
     canvas.style.setProperty(""height"", h + ""px"", ""important"");
    } else {
     canvas.style.removeProperty(""width"");
     canvas.style.removeProperty(""height"");
    }
   }
  }";

			string replaceUpdateCanvasString =
@"else {
   if (canvas.width != wNative) canvas.width = wNative;
   if (canvas.height != hNative) canvas.height = hNative;
   if (typeof canvas.style != ""undefined"") {
    if (w != wNative || h != hNative) {
     canvas.style.setProperty(""width"", w + ""px"", ""important"");
     canvas.style.setProperty(""height"", h + ""px"", ""important"");
    } else {
     //canvas.style.removeProperty(""width"");
     //canvas.style.removeProperty(""height"");
    }
   }
  }";

			source.Replace(findUpdateCanvasString, replaceUpdateCanvasString);
		}
	}

}
