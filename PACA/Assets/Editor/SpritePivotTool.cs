using UnityEditor;
using UnityEngine;
using UnityEditor.Sprites;
using UnityEditor.U2D.Sprites;

public class SpritePivotTool : EditorWindow
{
    private Texture2D targetTexture;
    private SpriteAlignment alignment = SpriteAlignment.Center;
    private Vector2 customPivot = new Vector2(0.47f, 0.455f);

    [MenuItem("Tools/Sprite/批量设置中心点 (Pivot)")]
    static void Init()
    {
        GetWindow<SpritePivotTool>("Sprite 批量中心点工具");
    }

    void OnGUI()
    {
        GUILayout.Label("选择多切片纹理 (Sprite Mode = Multiple)", EditorStyles.boldLabel);
        targetTexture = (Texture2D)EditorGUILayout.ObjectField("纹理", targetTexture, typeof(Texture2D), false);

        GUILayout.Space(10);
        alignment = (SpriteAlignment)EditorGUILayout.EnumPopup("预设对齐", alignment);

        if (alignment == SpriteAlignment.Custom)
        {
            customPivot = EditorGUILayout.Vector2Field("自定义 Pivot (0~1)", customPivot);
        }

        if (GUILayout.Button("应用到所有切片"))
        {
            ApplyPivotToAllSprites();
        }
    }

    void ApplyPivotToAllSprites()
    {
        if (targetTexture == null)
        {
            EditorUtility.DisplayDialog("提示", "请先选择纹理", "确定");
            return;
        }

        string path = AssetDatabase.GetAssetPath(targetTexture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null || importer.spriteImportMode != SpriteImportMode.Multiple)
        {
            EditorUtility.DisplayDialog("错误", "不是多切片纹理", "确定");
            return;
        }

        // 新版 API（2021+ 稳定）
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();
        var spriteRects = dataProvider.GetSpriteRects();

        foreach (var rect in spriteRects)
        {
            rect.alignment = alignment;
            if (alignment == SpriteAlignment.Custom)
                rect.pivot = customPivot;
        }

        dataProvider.SetSpriteRects(spriteRects);
        dataProvider.Apply();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        EditorUtility.DisplayDialog("完成", $"已修改 {spriteRects.Length} 个 Sprite 的中心点", "确定");
    }
}