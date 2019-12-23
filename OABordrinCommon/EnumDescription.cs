using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OABordrinCommon
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class EnumDescription : Attribute
    {
        private string enumDisplayText;
        private int enumRank;
        private bool isDefault;
        private bool _IsNotShow;
        private FieldInfo fieldIno;

        /// <summary>
        /// 描述枚举值
        /// </summary>
        /// <param name="enumDisplayText">描述内容</param>
        /// <param name="enumRank">排列顺序</param>
        public EnumDescription(string enumDisplayText, int enumRank)
        {
            this.enumDisplayText = enumDisplayText;
            this.enumRank = enumRank;
        }

        /// <summary>
        /// 描述枚举值，默认排序为5
        /// </summary>
        /// <param name="enumDisplayText">描述内容</param>
        public EnumDescription(string enumDisplayText)
            : this(enumDisplayText, 5) { }

        /// <summary>
        /// 描述枚举值，默认排序为5
        /// </summary>
        /// <param name="enumDisplayText">描述内容</param>
        public EnumDescription(string enumDisplayText, bool isdefault = false)
            : this(enumDisplayText, 5)
        {
            this.isDefault = isdefault;
        }

        public string EnumDisplayText
        {
            get { return this.enumDisplayText; }
        }

        public int EnumRank
        {
            get { return enumRank; }
        }

        private int? _EnumValue;
        public int EnumValue
        {
            set { _EnumValue = value; }
            get
            {
                if (_EnumValue == null)
                {
                    _EnumValue = (int)fieldIno.GetValue(null);
                }
                return (int)_EnumValue;
            }
        }

        public string FieldName
        {
            get { return fieldIno.Name; }
        }

        public bool IsNotShow
        {
            set
            {
                _IsNotShow = value;
            }
            get
            {
                return _IsNotShow;
            }
        }
        #region  =========================================对枚举描述属性的解释相关函数

        /// <summary>
        /// 排序类型
        /// </summary>
        public enum SortType
        {
            /// <summary>
            ///按枚举顺序默认排序
            /// </summary>
            Default,
            /// <summary>
            /// 按描述值排序
            /// </summary>
            DisplayText,
            /// <summary>
            /// 按排序熵
            /// </summary>
            Rank
        }

        private static System.Collections.Hashtable cachedEnum = new Hashtable();


        /// <summary>
        /// 得到对枚举的描述文本
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static string GetEnumText(Type enumType)
        {
            EnumDescription[] eds = (EnumDescription[])enumType.GetCustomAttributes(typeof(EnumDescription), false);
            if (eds.Length != 1) return string.Empty;
            return eds[0].EnumDisplayText;
        }

        /// <summary>
        /// 获得指定枚举类型中，指定值的描述文本。
        /// </summary>
        /// <param name="enumValue">枚举值，不要作任何类型转换</param>
        /// <returns>描述字符串</returns>
        public static string GetFieldText(object enumValue)
        {
            EnumDescription[] descriptions = GetFieldTexts(enumValue.GetType(), SortType.Default, true);
            foreach (EnumDescription ed in descriptions)
            {
                if (ed.fieldIno.Name == enumValue.ToString()) return ed.EnumDisplayText;
            }
            return string.Empty;
        }


        /// <summary>
        /// 得到枚举类型定义的所有文本，按定义的顺序返回
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="enumType">枚举类型</param>
        /// <returns>所有定义的文本</returns>
        public static List<EnumDescription> GetFieldTexts<T>(bool showAll = true) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            { throw new Exception("Type given must be an Enum"); }
            return new List<EnumDescription>(GetFieldTexts(enumType, SortType.Default, showAll));
        }

        /// <summary>
        /// 得到枚举类型定义的所有文本
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="enumType">枚举类型</param>
        /// <param name="sortType">指定排序类型</param>
        /// <returns>所有定义的文本</returns>
        public static EnumDescription[] GetFieldTexts(Type enumType, SortType sortType, bool showAll)
        {
            EnumDescription[] descriptions = null;
            //缓存中没有找到，通过反射获得字段的描述信息
            if (cachedEnum.Contains(enumType.FullName + showAll) == false)
            {
                FieldInfo[] fields = enumType.GetFields();
                ArrayList edAL = new ArrayList();
                foreach (FieldInfo fi in fields)
                {
                    object[] eds = fi.GetCustomAttributes(typeof(EnumDescription), false);
                    if (eds.Length != 1) continue;
                    ((EnumDescription)eds[0]).fieldIno = fi;
                    if (showAll)
                    {
                        edAL.Add(eds[0]);
                    }
                    else if (!((EnumDescription)eds[0]).IsNotShow)
                    {
                        edAL.Add(eds[0]);
                    }
                }

                cachedEnum.Add(enumType.FullName + showAll, (EnumDescription[])edAL.ToArray(typeof(EnumDescription)));
            }
            descriptions = (EnumDescription[])cachedEnum[enumType.FullName + showAll];
            if (descriptions.Length <= 0) throw new NotSupportedException("枚举类型[" + enumType.Name + "]未定义属性EnumValueDescription");

            //按指定的属性冒泡排序
            for (int m = 0; m < descriptions.Length; m++)
            {
                //默认就不排序了
                if (sortType == SortType.Default) break;

                for (int n = m; n < descriptions.Length; n++)
                {
                    EnumDescription temp;
                    bool swap = false;

                    switch (sortType)
                    {
                        case SortType.Default:
                            break;
                        case SortType.DisplayText:
                            if (string.Compare(descriptions[m].EnumDisplayText, descriptions[n].EnumDisplayText) > 0) swap = true;
                            break;
                        case SortType.Rank:
                            if (descriptions[m].EnumRank > descriptions[n].EnumRank) swap = true;
                            break;
                    }

                    if (swap)
                    {
                        temp = descriptions[m];
                        descriptions[m] = descriptions[n];
                        descriptions[n] = temp;
                    }
                }
            }

            return descriptions;
        }


        public static List<EnumDescription> GetFieldTextsAndNull<T>(bool showAll = true) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            { throw new Exception("Type given must be an Enum"); }
            List<EnumDescription> enums = new List<EnumDescription>();
            var enumArray = GetFieldTexts(enumType, SortType.Default, showAll);
            enums.Add(new EnumDescription("") { EnumValue = -1 });
            enums.AddRange(enumArray);
            return enums;
        }

        public static T? GetEnumByText<T>(string fieldText) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            { throw new Exception("Type given must be an Enum"); }
            var enums = GetFieldTexts<T>();
            int? enumValue = null;
            if (enums != null)
            {
                foreach (var e in enums)
                {
                    if (e.EnumDisplayText.Equals(fieldText))
                    {
                        enumValue = e.EnumValue;
                        break;
                    }
                }
            }
            Array enumValues = Enum.GetValues(enumType);
            if (enumValue != null)
            {
                foreach (var ev in enumValues)
                {
                    if ((int)ev == enumValue)
                    { return (T)ev; }
                }
            }
            return null;
        }



        public static T GetEnumByTextUseDefault<T>(string fieldText) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            { throw new Exception("Type given must be an Enum"); }
            var enums = GetFieldTexts<T>();
            int? enumValue = null;
            int? defalutValue = null;
            if (enums != null)
            {
                foreach (var e in enums)
                {
                    if (e.FieldName.Equals(fieldText))
                    {
                        enumValue = e.EnumValue;
                    }
                    if (e.isDefault)
                    {
                        defalutValue = e.EnumValue;
                    }
                }
            }
            Array enumValues = Enum.GetValues(enumType);
            if (enumValue != null)
            {
                foreach (var ev in enumValues)
                {
                    if ((int)ev == enumValue)
                    { return (T)ev; }
                }
            }
            if (defalutValue != null)
            {
                foreach (var ev in enumValues)
                {
                    if ((int)ev == defalutValue)
                    { return (T)ev; }
                }
            }
            return default(T);
        }

        public static T? GetEnumByValue<T>(int value) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            Array enumValues = Enum.GetValues(enumType);
            foreach (var ev in enumValues)
            {
                if ((int)ev == value)
                { return (T)ev; }
            }
            return null;
        }

        public static T? GetEnumByValueUseDefault<T>(int value) where T : struct, IConvertible
        {
            var enums = GetFieldTexts<T>();
            int? defalutValue = null;
            if (enums != null)
            {
                foreach (var e in enums)
                {
                    if (e.isDefault)
                    {
                        defalutValue = e.EnumValue;
                    }
                }
            }

            Type enumType = typeof(T);
            Array enumValues = Enum.GetValues(enumType);
            foreach (var ev in enumValues)
            {
                if ((int)ev == value)
                { return (T)ev; }
            }
            foreach (var ev in enumValues)
            {
                if ((int)ev == defalutValue)
                { return (T)ev; }
            }
            return null;
        }

        //public static IEnumerable<SelectListItem> GetSearchEnumList<T>(bool isShowEmpty = true, string emptyValue = "", string emptyText = "", bool isShowAll = true) where T : struct, IConvertible
        //{
        //    var enumDescriptions = EnumDescription.GetFieldTexts<T>(isShowAll);
        //    var selectList = new List<SelectListItem>();
        //    if (isShowEmpty)
        //    {
        //        selectList.Add(new SelectListItem() { Text = emptyText, Value = emptyValue });
        //    }
        //    selectList.AddRange(
        //          enumDescriptions.ToArray().Select(
        //              m => new SelectListItem() { Text = m.EnumDisplayText, Value = m.EnumValue.ToString() }));

        //    return selectList;
        //}


        /// <summary>
        /// 将枚举转换成Json字符串
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        //public static string ConvertEnumToJavascript<T>() where T : struct, IConvertible
        //{
        //    var t = typeof(T);
        //    if (!t.IsEnum) throw new Exception("Type must be an enumeration");
        //    Array values = Enum.GetValues(t);
        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    foreach (object o in values)
        //    {
        //        string name = Enum.GetName(t, o);
        //        dict.Add(name, Enum.Format(t, o, "D"));
        //    }
        //    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    return serializer.Serialize(dict);
        //}
        #endregion
    }
}