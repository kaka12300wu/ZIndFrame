using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class EditorTools
{
    [OnOpenAsset]
    public static bool OnOpenAsset( int instanceID, int line )
    {
        string path = AssetDatabase.GetAssetPath( instanceID );
        if (path.EndsWith( ".prefab" ))
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>( path );
            if (null != obj)
            {
                GameObject o = PrefabUtility.InstantiatePrefab( obj ) as GameObject;
                o.transform.SetAsLastSibling();
                Selection.activeObject = o;
                return true;
            }
            return false;
        }
        return false;
    }

    [MenuItem( "GameObject/Toogle active _`" )]
    static void ChangeActive()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject o in objs)
        {
            if (!IsHirachyGameObject( o )) continue;
            o.SetActive( !o.activeSelf );
        }
    }

    static void ChangeSibling( GameObject o, int offset )
    {
        if (null == o) return;
        Transform parent = o.transform.parent;
        int sibling = o.transform.GetSiblingIndex();
        if (sibling == 0 && offset == -1)
            return;
        if (null == parent)
        {
            o.transform.SetSiblingIndex( sibling + offset );
        }
        else
        {
            int siblingCount = parent.childCount;
            if (sibling == siblingCount - 1 && offset == 1)
                return;
            o.transform.SetSiblingIndex( sibling + offset );
        }
    }

    [MenuItem( "Edit/Move Up &UP" )]
    static void SiblingUp()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject o in objs)
        {
            if (IsAsset( o )) continue;
            ChangeSibling( o, -1 );
        }
    }

    [MenuItem( "Edit/Move Down &DOWN" )]
    static void SiblingDown()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject o in objs)
        {
            if (IsAsset( o )) continue;
            ChangeSibling( o, 1 );
        }
    }

    [MenuItem( "Edit/Move Up &UP", true )]
    static bool CheckSiblingUp()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject o in objs)
        {
            if (!IsHirachyGameObject( o ))
                return false;
        }
        return true;
    }

    [MenuItem( "Edit/Move Down &DOWN", true )]
    static bool CheckSiblingDown()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject o in objs)
        {
            if (!IsHirachyGameObject( o ))
                return false;
        }
        return true;
    }

    public static bool IsAsset( Object o )
    {
        return AssetDatabase.Contains( o );
    }

    public static bool IsHirachyGameObject( Object o )
    {
        return !IsAsset( o ) && o.GetType() == typeof( GameObject );
    }
}