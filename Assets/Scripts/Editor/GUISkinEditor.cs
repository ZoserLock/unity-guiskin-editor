using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GUISkin))]
public class GUISkinEditor : MultiWindowEditor
{
    [System.Flags]
    enum ControlFlags
    {
        Normal      = 0x1,
        Hover       = 0x2,
        Active      = 0x4,
        Focused     = 0x8,
        Background  = 0x10,
        Text        = 0x20,
        Custom      = 0x40,
        UseON       = 0x80,
        Misc        = 0x100,
        ALL         = 0xffff,
        Interactable = Normal | Hover | Active | Focused,
    }
    enum SelectionMode
    {
        Default = 0,
        Custom  = 1,
        Options = 2,
    }
    class Style
    {
        public string name;
        public ControlFlags flags;
        public GUIStyle style;

        public Style(string n,GUIStyle s, ControlFlags t)
        {
            name  = n;
            style = s;
            flags = t;
        }
    }

    private static int sLabelWitdh = 130;

    private Vector2 _mainScroll = new Vector2(0, 0);
    private Vector2 _auxScroll = new Vector2(0, 0);
    
    private GUISkin _guiSkin;

    private Style   [] _styles;

    private Style   _selectedStyle;

    private GUIStyle _clipboardStyle;
    private GUIStyle _clipboardSelectedStyle;

    private GUIStyle _currentCustomStyle;

    private string[] _defaultStylesOptionNames;
    private int      _selectedDefaultOptionIndex = 0;


    // Colors
    private Color _guiStyleAreaColor;


    private SelectionMode _selectionMode = SelectionMode.Default;

    public void OnEnable()
    {
        SetupMultiWindow(0.6f);

        _guiSkin = target as GUISkin;
        CreateData();

        _guiStyleAreaColor = new Color(1.0f, 0.95f, 0.875f);
    }

    void CreateData()
    {
         _defaultStylesOptionNames   = new string[20];
        _styles                      = new Style[20]; 

        _styles[0]  = new Style("Box"                               , _guiSkin.box                              , ControlFlags.Normal| ControlFlags.Text);
        _styles[1]  = new Style("Button"                            , _guiSkin.button                           , ControlFlags.Interactable | ControlFlags.Background | ControlFlags.Text);
        _styles[2]  = new Style("Toggle"                            , _guiSkin.toggle                           , ControlFlags.Interactable | ControlFlags.Background | ControlFlags.UseON | ControlFlags.Text);
        _styles[3]  = new Style("Label"                             , _guiSkin.label                            , ControlFlags.Interactable | ControlFlags.Background | ControlFlags.Text);
        _styles[4]  = new Style("TextField"                         , _guiSkin.textField                        , ControlFlags.Interactable | ControlFlags.Text);
        _styles[5]  = new Style("TextArea"                          , _guiSkin.textArea                         , ControlFlags.Interactable | ControlFlags.Text);
        _styles[6]  = new Style("Window"                            , _guiSkin.window                           , ControlFlags.Normal);
        _styles[7]  = new Style("Horizontal Slider"                 , _guiSkin.horizontalSlider                 , ControlFlags.Normal);
        _styles[8]  = new Style("Horizontal Slider Thumb"           , _guiSkin.horizontalSliderThumb            , ControlFlags.Normal);
        _styles[9]  = new Style("Vertical Slider"                   , _guiSkin.verticalSlider                   , ControlFlags.Normal);
        _styles[10] = new Style("Vertical Slider Thumb"             , _guiSkin.verticalSliderThumb              , ControlFlags.Normal);
        _styles[11] = new Style("ScrollView"                        , _guiSkin.scrollView                       , ControlFlags.Normal);
        _styles[12] = new Style("HorizontalScrollbar"               , _guiSkin.horizontalScrollbar              , ControlFlags.Normal);
        _styles[13] = new Style("HorizontalScrollbar Thumb"         , _guiSkin.horizontalScrollbarThumb         , ControlFlags.Normal);
        _styles[14] = new Style("HorizontalScrollbar Left Button"   , _guiSkin.horizontalScrollbarLeftButton    , ControlFlags.Normal);
        _styles[15] = new Style("HorizontalScrollbar Right Button"  , _guiSkin.horizontalScrollbarRightButton   , ControlFlags.Normal);
        _styles[16] = new Style("Vertical Scrollbar"                , _guiSkin.verticalScrollbar                , ControlFlags.Normal);
        _styles[17] = new Style("Vertical ScrollbarThumb"           , _guiSkin.verticalScrollbarThumb           , ControlFlags.Normal);
        _styles[18] = new Style("Vertical Scrollbar Up Button"      , _guiSkin.verticalScrollbarUpButton        , ControlFlags.Normal);
        _styles[19] = new Style("Vertical Scrollbar Down Button"    , _guiSkin.verticalScrollbarDownButton      , ControlFlags.Normal);

        for (int a=0;a<_styles.Length;++a)
        {
            _defaultStylesOptionNames[a] = _styles[a].name;
        }
        _selectedDefaultOptionIndex = 0;
    }

    protected override void OnMultiHeaderGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Space(5);
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.BeginHorizontal("HelpBox");
        GUILayout.FlexibleSpace();
        GUILayout.Label("GUISkin - " + _guiSkin.name,EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Ping", GUILayout.Width(40)))
        {
            EditorGUIUtility.PingObject(_guiSkin);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    protected override void OnMultiInspectorMainGUI(Rect totalArea)
    {
        GUILayout.BeginArea(totalArea);
        _mainScroll = GUILayout.BeginScrollView(_mainScroll, "HelpBox");

        GUILayout.BeginHorizontal("HelpBox");
        if(GUILayout.Toggle(_selectionMode == SelectionMode.Default, "Default Styles", "Button", GUILayout.Height(30)))
        {
            _selectionMode = SelectionMode.Default;
        }
        if (GUILayout.Toggle(_selectionMode== SelectionMode.Custom, "Custom Styles", "Button", GUILayout.Height(30)))
        {
            _selectionMode = SelectionMode.Custom;
        }
        if (GUILayout.Toggle(_selectionMode == SelectionMode.Options,"Options","Button", GUILayout.Height(30)))
        {
            _selectionMode = SelectionMode.Options;
        }
        GUILayout.EndHorizontal();
        if (_selectionMode == SelectionMode.Default)
        {
            GUILayout.BeginVertical("HelpBox");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected:", GUILayout.ExpandWidth(false));
            _selectedDefaultOptionIndex = EditorGUILayout.Popup(_selectedDefaultOptionIndex, _defaultStylesOptionNames, GUILayout.ExpandWidth(true));

            SelectItem(_selectedDefaultOptionIndex);

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Copy"))
            {
                _clipboardSelectedStyle = _selectedStyle.style;
                _clipboardStyle = new GUIStyle(_selectedStyle.style);
            }

            if (GUILayout.Button("Paste"))
            {
                if (_clipboardStyle != null)
                {
                    _selectedStyle.style = _clipboardStyle;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("HelpBox");
            if (_selectedDefaultOptionIndex < 20)
            {
                if (_selectedStyle != null)
                {
                    GUI.color = _guiStyleAreaColor;
                    DrawGUIStyle(_selectedStyle);
                    GUI.color = Color.white;
                }
            }

            GUILayout.EndVertical();
        }
        else if(_selectionMode == SelectionMode.Custom)
        {
            List<GUIStyle> deleteList = new List<GUIStyle>();


            for(int a=0;a<_guiSkin.customStyles.Length;++a)
            {
                GUIStyle style = _guiSkin.customStyles[a];
                if(style != null)
                {
                    GUILayout.BeginHorizontal("HelpBox");
                    if(GUILayout.Button("Select"))
                    {
                        if(_currentCustomStyle==null)
                        {
                            _currentCustomStyle = _guiSkin.customStyles[a];
                        }
                        else
                        {
                            _currentCustomStyle = null;
                        }
                    }
                    GUILayout.FlexibleSpace();
                    if (string.IsNullOrEmpty(style.name))
                    {
                        GUILayout.Label("Unnamed Style");
                    }
                    else
                    {
                        GUILayout.Label(style.name);
                    }
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Copy"))
                    {
                        _clipboardSelectedStyle = style;
                        _clipboardStyle = new GUIStyle(style);
                    }
                    if (GUILayout.Button("Paste"))
                    {
                        _guiSkin.customStyles[a] = _clipboardStyle;
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        deleteList.Add(style);
                    }
                    GUILayout.EndHorizontal();
                    if(_currentCustomStyle==style)
                    {
                        GUILayout.BeginVertical("HelpBox");
                        GUI.color = _guiStyleAreaColor;
                        DrawGUIStyle(new Style("",_currentCustomStyle,ControlFlags.ALL));

                        GUI.color = Color.white;
                        GUILayout.EndVertical();
                    }
                }
            }
     
            if (deleteList.Count>0)
            {
                Undo.RecordObject(_guiSkin, "Custom GuiStyle Removed");
                List<GUIStyle> customStyles = new List<GUIStyle>(_guiSkin.customStyles);
                foreach (var style in deleteList)
                {
                    if(style == _currentCustomStyle)
                    {
                        _currentCustomStyle = null;
                    }
                    customStyles.Remove(style);

                }
                _guiSkin.customStyles = customStyles.ToArray();
                EditorUtility.SetDirty(_guiSkin);
            }
 
            GUILayout.BeginHorizontal("HelpBox");
            if (GUILayout.Button("Add New"))
            {
                Undo.RecordObject(_guiSkin, "Custom GuiStyle Added");
                List<GUIStyle> styles = new List<GUIStyle>(_guiSkin.customStyles);
                styles.Add(new GUIStyle());
                _guiSkin.customStyles = styles.ToArray();
                EditorUtility.SetDirty(_guiSkin);

            }
            GUILayout.EndHorizontal();

        }
        else if (_selectionMode == SelectionMode.Options)
        {
            GUILayout.BeginVertical("HelpBox");
            _guiSkin.settings.doubleClickSelectsWord = BoolField(_guiSkin.settings.doubleClickSelectsWord, "2 Click Select Word");
            _guiSkin.settings.tripleClickSelectsLine = BoolField(_guiSkin.settings.tripleClickSelectsLine, "3 Click Select Line");
            _guiSkin.settings.cursorColor = ColorField(_guiSkin.settings.cursorColor, "Cursor Color");
            _guiSkin.settings.cursorFlashSpeed = FloatField(_guiSkin.settings.cursorFlashSpeed, "Cursor Flash Speed");
            _guiSkin.settings.selectionColor = ColorField(_guiSkin.settings.selectionColor, "Selection Color");
            GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

    }
    protected override void OnMultiInspectorAuxGUI(Rect totalArea)
    {
        GUILayout.BeginArea(totalArea);
        _auxScroll = GUILayout.BeginScrollView(_auxScroll, "HelpBox");
        GUILayout.BeginVertical("HelpBox");
        GUILayout.BeginHorizontal("HelpBox");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Preview");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (_selectionMode == SelectionMode.Default || _selectionMode == SelectionMode.Options)
        {
            GUILayout.BeginVertical("HelpBox");

            GUISkin defaultSkin = GUI.skin;
            GUI.skin = target as GUISkin;

            DrawPreviewControls();

            GUI.skin = defaultSkin;

            GUILayout.EndVertical();
        }else
        {
            if (_currentCustomStyle != null)
            {
      
                GUILayout.BeginVertical("HelpBox");

                DrawPreviewControls(_currentCustomStyle);

                GUILayout.EndVertical();
           
            }
            else
            {
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.FlexibleSpace();
                GUILayout.Label("Select a custom style to preview");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    void SelectItem(int item)
    {
        if (item >= 0 && item < _styles.Length)
        {
            _selectedStyle = _styles[item];

        }
    }

    void DrawGUIStyle(Style inStyle)
    {
        GUIStyle style = inStyle.style;

        GUILayout.BeginHorizontal("HelpBox");
        style.name = TextField(style.name,"Name");
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("States", EditorStyles.boldLabel);
        GUILayout.Label("Background");
        GUILayout.BeginVertical("HelpBox");
        if ((inStyle.flags&ControlFlags.Normal)!=0)
            style.normal.background = ObjectField<Texture2D>(style.normal.background  , "Normal");

        if ((inStyle.flags & ControlFlags.Hover) != 0)
            style.hover.background = ObjectField<Texture2D>(style.hover.background   , "Hover");

        if ((inStyle.flags & ControlFlags.Active) != 0)
            style.active.background = ObjectField<Texture2D>(style.active.background  , "Active");

        if ((inStyle.flags & ControlFlags.Focused) != 0)
            style.focused.background = ObjectField<Texture2D>(style.focused.background , "Focus");

       
        if ((inStyle.flags & ControlFlags.UseON) != 0)
        {
            GUILayout.Space(5);

            if ((inStyle.flags & ControlFlags.Normal) != 0)
                style.onNormal.background = ObjectField<Texture2D>(style.onNormal.background, "On Normal");

            if ((inStyle.flags & ControlFlags.Hover) != 0)
                style.onHover.background = ObjectField<Texture2D>(style.onHover.background, "On Hover");

            if ((inStyle.flags & ControlFlags.Active) != 0)
                style.onActive.background = ObjectField<Texture2D>(style.onActive.background, "On Active");

            if ((inStyle.flags & ControlFlags.Focused) != 0)
                style.onFocused.background = ObjectField<Texture2D>(style.onFocused.background, "On Focus");
        }
        GUILayout.EndVertical();

        GUILayout.Label("TextColor");
        GUILayout.BeginVertical("HelpBox");

        if ((inStyle.flags & ControlFlags.Normal) != 0)
            style.normal.textColor = ColorField(style.normal.textColor, "Normal Color");

        if ((inStyle.flags & ControlFlags.Hover) != 0)
            style.hover.textColor = ColorField(style.hover.textColor, "Hover Color");

        if ((inStyle.flags & ControlFlags.Active) != 0)
            style.active.textColor = ColorField(style.active.textColor, "Active Color");

        if ((inStyle.flags & ControlFlags.Focused) != 0)
            style.focused.textColor = ColorField(style.focused.textColor, "Focus Color");

        if ((inStyle.flags & ControlFlags.UseON) != 0)
        {
            GUILayout.Space(5);

            if ((inStyle.flags & ControlFlags.Normal) != 0)
                style.onNormal.textColor = ColorField(style.onNormal.textColor, "On Normal Color");

            if ((inStyle.flags & ControlFlags.Hover) != 0)
                style.onHover.textColor = ColorField(style.onHover.textColor, "On Hover Color");

            if ((inStyle.flags & ControlFlags.Active) != 0)
                style.onActive.textColor = ColorField(style.onActive.textColor, "On Active Color");

            if ((inStyle.flags & ControlFlags.Focused) != 0)
                style.onFocused.textColor = ColorField(style.onFocused.textColor, "On Focus Color");
        }
        GUILayout.EndVertical();

        GUILayout.Label("Offsets",EditorStyles.boldLabel);
        GUILayout.BeginVertical("HelpBox");

        style.border    = RectField(style.border, "Border");
        style.margin    = RectField(style.margin, "Margin");
        style.padding   = RectField(style.padding, "Padding");
        style.overflow  = RectField(style.overflow, "Overflow");
        GUILayout.EndVertical();

        GUILayout.Label("Text", EditorStyles.boldLabel);
        GUILayout.BeginVertical("HelpBox");

        style.font      = ObjectField<Font>(style.font, "Font");
        style.fontSize  = IntField(style.fontSize, "Font Size");
        style.fontStyle = (FontStyle)EnumField(style.fontStyle, "Font Style");
        style.alignment = (TextAnchor)EnumField(style.alignment, "Alignment");
        style.clipping  = (TextClipping)EnumField(style.clipping, "Clipping");

        style.wordWrap = BoolField(style.wordWrap, "Word Wrap");
        style.richText = BoolField(style.richText, "Rich Text");

        GUILayout.EndVertical();
        GUILayout.Label("Misc", EditorStyles.boldLabel);
        GUILayout.BeginVertical("HelpBox");
        style.imagePosition = (ImagePosition)EnumField(style.imagePosition, "Image Position");
        style.contentOffset = Vector2Field(style.contentOffset, "Content Offset");
        style.fixedWidth = FloatField(style.fixedWidth, "Fixed Width");
        style.fixedHeight = FloatField(style.fixedHeight, "Fixed Height");
        style.stretchWidth = BoolField(style.stretchWidth, "Stretch Width");
        style.stretchHeight = BoolField(style.stretchHeight, "Stretch Height");
        GUILayout.EndVertical();
        GUILayout.EndVertical();

    }
    void DrawPreviewControls(GUIStyle style)
    {
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Boxes");
        GUILayout.BeginHorizontal();
        GUILayout.Box("Box", style, GUILayout.Height(50));
        GUILayout.Box("Box", style, GUILayout.Height(40));
        GUILayout.Box("Box", style, GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Buttons");
        GUILayout.BeginHorizontal();
        GUILayout.Button("Button", style, GUILayout.Height(50));
        GUILayout.Button("Button", style, GUILayout.Height(40));
        GUILayout.Button("Button", style, GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Toggles");
        GUILayout.BeginHorizontal();
        GUILayout.Toggle(true, "Toggle", style);
        GUILayout.Toggle(false, "Toggle", style);
        GUILayout.Toggle(true, "Toggle", style);
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Labels");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Label", style);
        GUILayout.Label("Label Medium", style);
        GUILayout.Label("Label Big", style);
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Text Fields");
        GUILayout.BeginHorizontal();
        GUILayout.TextField("TextField", style, GUILayout.Height(50));
        GUILayout.TextField("TextField Medium\n2 lines", style, GUILayout.Height(40));
        GUILayout.TextField("TextField Small", style, GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Text Areas");
        GUILayout.BeginHorizontal();
        GUILayout.TextArea("TextArea", style, GUILayout.Height(50));
        GUILayout.TextArea("TextArea Medium\n2 Lines", style, GUILayout.Height(40));
        GUILayout.TextArea("TextArea Small", style, GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    void DrawPreviewControls()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Boxes");
        GUILayout.BeginHorizontal();
        GUILayout.Box("Box", GUILayout.Height(50));
        GUILayout.Box("Box", GUILayout.Height(40));
        GUILayout.Box("Box", GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Buttons");
        GUILayout.BeginHorizontal();
        GUILayout.Button("Button", GUILayout.Height(50));
        GUILayout.Button("Button", GUILayout.Height(40));
        GUILayout.Button("Button", GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Toggles");
        GUILayout.BeginHorizontal();
        GUILayout.Toggle(true, "Toggle");
        GUILayout.Toggle(false, "Toggle");
        GUILayout.Toggle(true, "Toggle");
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Labels");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Label");
        GUILayout.Label("Label Medium");
        GUILayout.Label("Label Big");
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Text Fields");
        GUILayout.BeginHorizontal();
        GUILayout.TextField("TextField", GUILayout.Height(50));
        GUILayout.TextField("TextField Medium\n2 lines", GUILayout.Height(40));
        GUILayout.TextField("TextField Small", GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Text Areas");
        GUILayout.BeginHorizontal();
        GUILayout.TextArea("TextArea", GUILayout.Height(50));
        GUILayout.TextArea("TextArea Medium\n2 Lines", GUILayout.Height(40));
        GUILayout.TextArea("TextArea Small", GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Scroll View");
        GUILayout.BeginHorizontal();
        GUILayout.BeginScrollView(new Vector2(0,0), GUILayout.Height(70));
        GUILayout.EndScrollView();
        GUILayout.BeginScrollView(new Vector2(0, 0), GUILayout.Height(50));
        GUILayout.EndScrollView();
        GUILayout.BeginScrollView(new Vector2(0, 0), GUILayout.Height(40));
        GUILayout.EndScrollView();

        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Horizontal Slider");
        GUILayout.BeginHorizontal();
        GUILayout.HorizontalSlider(0.5f,1,0, GUILayout.Height(20));
        GUILayout.HorizontalSlider(0.75f, 1, 0, GUILayout.Height(20));
        GUILayout.HorizontalSlider(0.25f, 1, 0, GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Vertical Slider");
        GUILayout.BeginHorizontal();
        GUILayout.VerticalSlider(0.5f, 1, 0, GUILayout.Height(100));
        GUILayout.VerticalSlider(0.75f, 1, 0, GUILayout.Height(80));
        GUILayout.VerticalSlider(0.25f, 1, 0, GUILayout.Height(50));
        GUILayout.VerticalSlider(0.0f, 1, 0, GUILayout.Height(100));
        GUILayout.VerticalSlider(0.15f, 1, 0, GUILayout.Height(80));
        GUILayout.VerticalSlider(0.85f, 1, 0, GUILayout.Height(50));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Horizontal ScrollBar");
        GUILayout.BeginHorizontal();
        GUILayout.HorizontalScrollbar(0.5f,0, 1, 0, GUILayout.Height(20));
        GUILayout.HorizontalScrollbar(0.75f, 0, 1, 0, GUILayout.Height(20));
        GUILayout.HorizontalScrollbar(0.25f, 0, 1, 0, GUILayout.Height(20));
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Vertical ScrollBar");
        GUILayout.BeginHorizontal();
        GUILayout.VerticalScrollbar(0.5f, 0, 1, 0, GUILayout.Height(100));
        GUILayout.VerticalScrollbar(0.75f, 0, 1, 0, GUILayout.Height(80));
        GUILayout.VerticalScrollbar(0.25f, 0, 1, 0, GUILayout.Height(50));
        GUILayout.VerticalScrollbar(0.0f, 0, 1, 0, GUILayout.Height(100));
        GUILayout.VerticalScrollbar(0.15f, 0, 1, 0, GUILayout.Height(80));
        GUILayout.VerticalScrollbar(0.95f, 0, 1, 0, GUILayout.Height(50));
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

    }

    #region Editor Fields
    T ObjectField<T>(T obj, string title) where T : Object
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(GUISkinEditor.sLabelWitdh));
        T newObject = EditorGUILayout.ObjectField(obj, typeof(T),false) as T;
        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin " + title + " Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newObject;
        }

        return obj;
    }

    Color ColorField(Color color, string title)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(GUISkinEditor.sLabelWitdh));

        Color newColor = EditorGUILayout.ColorField(color);
        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin " + title + " Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newColor;
        }

        return color;
    }

    RectOffset RectField(RectOffset rect, string title)
    {
        RectOffset newRect = new RectOffset(rect.left,rect.right,rect.top,rect.bottom);

        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(GUISkinEditor.sLabelWitdh));

        newRect.left = EditorGUILayout.IntField(rect.left);
        newRect.top = EditorGUILayout.IntField(rect.top);
        newRect.right = EditorGUILayout.IntField(rect.right);
        newRect.bottom = EditorGUILayout.IntField(rect.bottom);

        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin " + title + " Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newRect;
        }

        return rect;
    }

    string TextField(string text,string title)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title,GUILayout.Width(GUISkinEditor.sLabelWitdh));
        string newText = GUILayout.TextField(text);
        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin "+title+" Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newText;
        }

        return text;
    }

    System.Enum EnumField(System.Enum enumType,string title)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(GUISkinEditor.sLabelWitdh));

        System.Enum newNumber = EditorGUILayout.EnumPopup(enumType);

        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin " + title + " Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newNumber;
        }

        return enumType;
    }

    int IntField(int number, string title)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(GUISkinEditor.sLabelWitdh));
        int newNumber = EditorGUILayout.IntField(number);
       
        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin " + title + " Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newNumber;
        }

        return number;
    }

    float FloatField(float number, string title)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(GUISkinEditor.sLabelWitdh));
        float newNumber = EditorGUILayout.FloatField(number);

        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin " + title + " Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newNumber;
        }

        return number;
    }

    bool BoolField(bool value, string title)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(GUISkinEditor.sLabelWitdh));
        bool newValue = EditorGUILayout.Toggle(value);

        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin " + title + " Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newValue;
        }

        return value;
    }

    Vector2 Vector2Field(Vector2 value, string title)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(GUISkinEditor.sLabelWitdh));
        Vector2 newValue = EditorGUILayout.Vector2Field("",value);

        GUILayout.EndHorizontal();
        if (GUI.changed)
        {
            Undo.RecordObject(_guiSkin, "GUISkin " + title + " Changed");
            EditorUtility.SetDirty(_guiSkin);
            return newValue;
        }

        return value;
    }
    #endregion
}
