using Malee;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Data/Table Description" )]
public class TableDescription : ScriptableObject
// Author: Christopher Chamberlain - 2017
{
    [SerializeField, Reorderable]
    private FieldList m_Fields;

    [Serializable]
    private class FieldList : ReorderableArray<Field>
    { }

    public IList<Field> Fields { get { return m_Fields; } }

    [Serializable]
    public class Field
    {
        /// <summary>
        /// Column/field name.
        /// </summary>
        [Tooltip( "Column name." )]
        public string Name;

        /// <summary>
        /// Column/field type.
        /// </summary>
        [Tooltip( "Column type." )]
        public FieldType Type;
    }

    public enum FieldType
    {
        String,
        Number
    }
}