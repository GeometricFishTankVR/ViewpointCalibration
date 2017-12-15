using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
 
/// <summary>
/// An object that attempts to serialize dictionary-like elements in a way that works for the unity inspector. <para/>
/// Extend <see cref="SerialAssetMap{TKey, TVal}"/>, not this non-generic class.
/// </summary>
public abstract class SerialAssetMap : ScriptableObject, ISerializationCallbackReceiver
// Author: Christopher Chamberlain - 2017
{
    [SerializeField]
    private List<Item> Items;

    [SerializeField]
    private string KeyTypeName;
    private Type KeyType;

    [SerializeField]
    private string ObjTypeName;
    private Type ObjType;

    public SerialAssetMap( Type keyType, Type objType )
    {
        // 
        KeyType = keyType;
        ObjType = objType;

        //
        ObjTypeName = ObjType.AssemblyQualifiedName;
        KeyTypeName = KeyType.AssemblyQualifiedName;

        // 
        if( KeyType.IsEnum ) InitializeEnumSet( KeyType );
        else
        {
            throw new NotImplementedException( "Non-enum keys not yet implemented" );
        }
    }

    #region Get Default / Enum Values

    protected static object GetDefault( Type type )
    {
        if( type.IsValueType ) return Activator.CreateInstance( type );
        else return null;
    }

    protected static Type GetDefault<Type>()
    {
        return (Type) GetDefault( typeof( Type ) );
    }

    protected static TEnum[] GetEnumValues<TEnum>()
    {
        var type = typeof( TEnum );

        if( type.IsEnum == false )
            throw new InvalidOperationException( "Unable to get enum values from non enum type." );

        // 
        return (TEnum[]) Enum.GetValues( type );
    }

    #endregion

    #region Get/Find

    protected IEnumerable<object> GetKeys()
    {
        return Items.Select( item => item.Key );
    }

    protected IEnumerable<object> GetValues()
    {
        return Items.Select( item => item.Object );
    }

    protected bool FindByKey( object key, out object val )
    {
        var idx = Items.FindIndex( item => Equals( item.Key, key ) );

        if( idx >= 0 )
        {
            val = Items[idx].Object;
            return true;
        }
        else
        {
            val = GetDefault( ObjType );
            return false;
        }
    }

    #endregion

    private void SyncKeys()
    {
        if( KeyType.IsEnum )
        {
            // 
            var newNames = (int[]) Enum.GetValues( KeyType );
            var oldNames = Items.Select( x => (int) x.Key ).ToArray();

            // Remove difference set
            foreach( var invalid in oldNames.Except( newNames ) )
                Items.RemoveAll( x => Equals( x.Key, invalid ) );
        }
        else
        {
            // Do Nothing?
        }
    }

    protected void InitializeEnumSet( Type type )
    {
        // 
        var enums = Enum.GetValues( type );

        //
        Items = new List<Item>();
        for( int i = 0; i < enums.Length; i++ )
        {
            // Create new entry
            var item = new Item( this );
            item.Object = GetDefault( ObjType );
            item.Key = enums.GetValue( i );

            // 
            Items.Add( item );
        }
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        KeyType = Type.GetType( KeyTypeName );
        ObjType = Type.GetType( ObjTypeName );
    }

    [Serializable]
    private class Item : ISerializationCallbackReceiver
    {
        [SerializeField]
        private SerialAssetMap Map;

        [SerializeField]
        private string KeyJson;

        [SerializeField]
        private string ObjectJson;

        [SerializeField]
        private UnityEngine.Object GameObject;

        // 
        public object Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        // 
        public object Object
        {
            get
            {
                if( IsUnityObject ) return GameObject;
                else return _Object;
            }

            set
            {
                if( IsUnityObject ) GameObject = ( value as UnityEngine.Object );
                else _Object = value;
            }
        }

        private bool IsUnityObject
        {
            get
            {
                if( _Object is UnityEngine.Object ) return true;
                else
                {
                    return Map.ObjType.IsSubclassOf( typeof( UnityEngine.Object ) );
                    // return Map.ObjType.IsAssignableFrom( typeof( UnityEngine.Object ) );
                }
            }
        }

        // 
        private object _Object;
        private object _Key;

        public Item( SerialAssetMap map )
        {
            Map = map;
        }

        public void OnBeforeSerialize()
        {
            // Convert key to string representation
            KeyJson = SerialzieSystemType( _Key, Map.KeyType );

            // Convert object to string representation
            if( IsUnityObject == false )
                ObjectJson = SerialzieSystemType( _Object, Map.ObjType );
        }

        public void OnAfterDeserialize()
        {
            // Convery string representation to key
            _Key = DeserialzieSystemType( KeyJson, Map.KeyType );

            // Convery string representation to key
            if( IsUnityObject == false )
                _Object = DeserialzieSystemType( ObjectJson, Map.ObjType );
        }

        #region Simple serialization

        private static object DeserialzieSystemType( string text, Type type )
        {
            if( type.IsPrimitive )
            {
                // Encoded primitive as string, convert to primitive.
                var converter = TypeDescriptor.GetConverter( type );
                return converter.ConvertFrom( text );
            }
            else
            {
                // Encoded object type as JSON, parse json
                return JsonUtility.FromJson( text, type );
            }
        }

        private static string SerialzieSystemType( object obj, Type type )
        {
            if( type.IsPrimitive )
            {
                // Store primitive with string encoding
                return obj.ToString();
            }
            else
            {
                // Store complex object as JSON representation
                return JsonUtility.ToJson( obj );
            }
        }

        #endregion
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor( typeof( SerialAssetMap ), true )]
    public class EnumCollectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // DrawDefaultInspector();

            var col = target as SerialAssetMap;
            if( col == null ) return;

            // EditorGUILayout.HelpBox( string.Format( "Value Collection of \"{0}\" indexed by \"{1}\".", col.ObjType, col.KeyType ), MessageType.None );
            // EditorGUILayout.HelpBox( string.Format( "Key: {1} Value: {0}.", col.ObjType, col.KeyType ), MessageType.None );

            // 
            foreach( var item in col.Items )
            {
                var key = item.Key;
                var obj = item.Object;

                // 
                col.SyncKeys();

                var name = key.ToString();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel( name );

                // UnityEngine.Object 
                if( col.ObjType.IsSubclassOf( typeof( UnityEngine.Object ) ) )
                {
                    var old = item.Object as UnityEngine.Object;
                    var val = EditorGUILayout.ObjectField( old, col.ObjType, allowSceneObjects: false );
                    if( old != val ) item.Object = val;
                }
                else
                // UnityEngine.Color
                if( ( col.ObjType == typeof( Color ) ) ||
                    ( col.ObjType == typeof( Color32 ) ) )
                {
                    var old = Cast<Color>( item.Object );
                    var val = EditorGUILayout.ColorField( old );
                    if( old != val ) item.Object = val;
                }
                else
                // UnityEngine.Bounds
                if( ( col.ObjType == typeof( Bounds ) ) )
                {
                    var old = Cast<Bounds>( item.Object );
                    var val = EditorGUILayout.BoundsField( old );
                    if( old != val ) item.Object = val;
                }
                else
                // UnityEngine.Vector2
                if( ( col.ObjType == typeof( Vector2 ) ) )
                {
                    var old = Cast<Vector2>( item.Object );
                    var val = EditorGUILayout.Vector2Field( string.Empty, old );
                    if( old != val ) item.Object = val;
                }
                else
                // UnityEngine.Vector3
                if( ( col.ObjType == typeof( Vector3 ) ) )
                {
                    var old = Cast<Vector3>( item.Object );
                    var val = EditorGUILayout.Vector3Field( string.Empty, old );
                    if( old != val ) item.Object = val;
                }
                else
                // UnityEngine.Vector4
                if( ( col.ObjType == typeof( Vector4 ) ) )
                {
                    var old = Cast<Vector4>( item.Object );
                    var val = EditorGUILayout.Vector4Field( string.Empty, old );
                    if( old != val ) item.Object = val;
                }
                else
                // System.Single
                if( ( col.ObjType == typeof( Single ) ) )
                {
                    var old = Cast<Single>( item.Object );
                    var val = EditorGUILayout.FloatField( old );
                    if( old != val ) item.Object = val;
                }
                else
                // System.Double
                if( ( col.ObjType == typeof( Double ) ) )
                {
                    var old = Cast<Double>( item.Object );
                    var val = EditorGUILayout.DoubleField( old );
                    if( old != val ) item.Object = val;
                }
                else
                // System.Int32
                if( ( col.ObjType == typeof( Int32 ) ) )
                {
                    var old = Cast<Int32>( item.Object );
                    var val = EditorGUILayout.IntField( old );
                    if( old != val ) item.Object = val;
                }
                else
                // System.Int64
                if( ( col.ObjType == typeof( Int64 ) ) )
                {
                    var old = Cast<Int64>( item.Object );
                    var val = EditorGUILayout.LongField( old );
                    if( old != val ) item.Object = val;
                }
                else
                // System.Boolean
                if( ( col.ObjType == typeof( Boolean ) ) )
                {
                    var old = Cast<Boolean>( item.Object );
                    var val = EditorGUILayout.Toggle( old );
                    if( old != val ) item.Object = val;
                }
                else
                // Enum
                if( ( col.ObjType.IsEnum ) )
                {
                    var old = Cast<Enum>( item.Object );
                    var val = EditorGUILayout.EnumPopup( old );
                    if( old != val ) item.Object = val;
                }
                else
                // String
                if( col.ObjType == typeof( string ) )
                {
                    var old = Cast<string>( item.Object );
                    var val = EditorGUILayout.TextArea( old );
                    if( old != val ) item.Object = val;
                }
                else
                {
                    // 
                    EditorGUILayout.LabelField( string.Format( "Unknown Inspector Type" ) );
                }

                EditorGUILayout.EndHorizontal();
            }

            // 
            EditorUtility.SetDirty( col );
        }

        private T Cast<T>( object @object )
        {
            if( @object == null ) return default( T );
            else return (T) @object;
        }
    }

#endif
}

/// <summary>
/// An object that attempts to serialize dictionary-like elements in a way that works for the unity inspector. 
/// Probably buggy.
/// </summary>
public abstract class SerialAssetMap<TKey, TVal> : SerialAssetMap
// Author: Christopher Chamberlain - 2017
{
    protected SerialAssetMap()
        : base( typeof( TKey ), typeof( TVal ) )
    { }

    public IEnumerable<TKey> Keys { get { return GetKeys().Cast<TKey>(); } }

    public IEnumerable<TVal> Values { get { return GetKeys().Cast<TVal>(); } }

    public TVal this[TKey key]
    {
        get { return Get( key ); }
    }

    public TVal Get( TKey key )
    {
        object val;
        if( FindByKey( key, out val ) ) return (TVal) val;
        else return GetDefault<TVal>();
    }
}