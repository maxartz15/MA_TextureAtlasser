#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MA_TextureAtlasserPro
{
    public class MA_TextureAtlasserProWindow : EditorWindow 
    {
        public static MA_TextureAtlasserProWindow thisWindow;
		public MA_TextureAtlasserProSettings settings;
        public MA_TextureAtlasserProAtlas textureAtlas;

        public MA_TextureAtlasserProWorkView workView;
		public MA_TextureAtlasserProMenuView menuView;
		public MA_TextureAtlasserProInspectorView inspectorView;
		public MA_TextureAtlasserProDebugView debugView;

		private static bool isLoaded = false;		//Make sure we wait a frame at the start to setup and don't draw.

        [MenuItem("MA_ToolKit/MA_TextureAtlasserPro/Atlas Editor")]
        private static void Init()
        {
            GetCurrentWindow();

            thisWindow.titleContent.text = "MA_TextureAtlasserPro";
			thisWindow.minSize = new Vector2(375, 360);
            thisWindow.wantsMouseMove = true;
			
			thisWindow.Show();
        }

		private void OnEnable()
		{
			//Load the icons
			//MA_TextureAtlasserProGuiLoader.LoadEditorGui(thisWindow.settings.editorGuiSettings);
		}

        private static void GetCurrentWindow()
		{
			thisWindow = (MA_TextureAtlasserProWindow)EditorWindow.GetWindow<MA_TextureAtlasserProWindow>();
		}

        private void CloseCurrentWindow()
		{
			if(thisWindow == null)
			{
				GetCurrentWindow();
			}
			thisWindow.Close();
		}

        private static void CreateViews()
		{
			if(thisWindow == null)
			{
				GetCurrentWindow();
			}

			thisWindow.settings = MA_TextureAtlasserProUtils.LoadSettings();
			MA_TextureAtlasserProGuiLoader.LoadEditorGui(thisWindow.settings);
			thisWindow.workView = new MA_TextureAtlasserProWorkView(thisWindow, "workView");
			thisWindow.menuView = new MA_TextureAtlasserProMenuView(thisWindow, "menuView");
			thisWindow.inspectorView = new MA_TextureAtlasserProInspectorView(thisWindow, "inspectorView");
			thisWindow.debugView = new MA_TextureAtlasserProDebugView(thisWindow, "debugView");
		}

        private Event ProcessEvents()
		{
			Event e = Event.current;

			return e;
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnReload()
		{
			//Make sure that when the compiler is finished and reloads the scripts, we are waiting for the next Event.
			isLoaded = false;
		}

        private void OnGUI()
        {
            //Check window
			if(thisWindow == null)
			{
				GetCurrentWindow();
				return;
			}
			
			//Check views
			if(settings == null || workView == null || menuView == null || inspectorView == null || debugView == null)
			{
				CreateViews();
				return;
			}

            //Get current event
            Event e = ProcessEvents();

            //Calculate view rects
            Rect workViewRect = new Rect(position.width - position.width, position.height - position.height, position.width, position.height);
			Rect debugViewRect = new Rect(position.width - MA_TextureAtlasserProUtils.VIEW_OFFSET - 164, position.height - MA_TextureAtlasserProUtils.VIEW_OFFSET - 22, 164, 22);
            Rect menuViewRect = new Rect(MA_TextureAtlasserProUtils.VIEW_OFFSET, MA_TextureAtlasserProUtils.VIEW_OFFSET, position.width - (MA_TextureAtlasserProUtils.VIEW_OFFSET * 2), 44);
            Rect inspectorViewRect = new Rect(MA_TextureAtlasserProUtils.VIEW_OFFSET, menuViewRect.y + menuViewRect.height + MA_TextureAtlasserProUtils.VIEW_OFFSET, 164, position.height - menuViewRect.height - (MA_TextureAtlasserProUtils.VIEW_OFFSET * 3));

            //Draw views and windows in the right oder from back to front
			if(isLoaded)
			{			
				workView.UpdateView(e, workViewRect);	
				debugView.UpdateView(e, debugViewRect);	
				inspectorView.UpdateView(e, inspectorViewRect);
				menuView.UpdateView(e, menuViewRect);
			}
	
			Repaint();

			if(e.type == EventType.Repaint)
				isLoaded = true;
        }
    }
}
#endif