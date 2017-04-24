using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSAC.Model
{
    public class AudioControllerData : ScriptableObject
    {
        public CategoryItem[] items;
        public bool foldOutCategories = false;

        public CategoryItem getCategoryItem(string name)
        {
            return System.Array.Find(items, (x) =>
            {
                return x.name == name;
            });
        }
    }
}