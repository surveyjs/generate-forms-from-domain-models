﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using SurveyJSAsFormLibrary.Attributes;
using SurveyJSAsFormLibrary.DomainModels;

namespace SurveyJSAsFormLibrary.Code
{
    public class JSONGeneratorByModelClass
    {
        Type type;
        public JSONGeneratorByModelClass(Type type)
        {
            this.type = type;
        }
        public dynamic Generate()
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            this.fillProperties(properties, this.type);
            if (properties.Count == 0) return new { };
            var pages = new List<dynamic>();
            var elements = new List<dynamic>();
            CustomAttributeData curPageAttr = getFormPageAttribute(properties[0]);
            for(int i = 0; i < properties.Count; i ++)
            {
                var prop = properties[i];
                if (i > 0)
                {
                    CustomAttributeData pageAttr = getFormPageAttribute(prop);
                    if (pageAttr != null)
                    {
                        pages.Add(new { title = this.getAttributeValueByProp(curPageAttr, "Title"), elements = elements });
                        elements = new List<dynamic>();
                        curPageAttr = pageAttr;
                    }
                }
                var element = this.createElementByProp(prop);
                if(element != null)
                {
                    elements.Add(element);
                }
            }
            if (elements.Count > 0)
            {
                pages.Add(new { title = this.getAttributeValueByProp(curPageAttr, "Title"), elements = elements });
            }
            var json = new ExpandoObject();
            json.TryAdd("pages", pages);
            return json;
        }
        private void fillProperties(List<PropertyInfo> properties, Type type)
        {
            Type parentType = type.BaseType;
            if(parentType != null && typeof(DomainModel).IsAssignableFrom(parentType))
            {
                fillProperties(properties, parentType);
            }
            properties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        }
        private dynamic createElementByProp(PropertyInfo prop)
        {
            var el = new ExpandoObject();
            el.TryAdd("name", prop.Name);
            this.setElementAttrByPropType(el, prop);
            foreach (var attr in prop.CustomAttributes)
            {
                if (attr.AttributeType == typeof(RequiredAttribute))
                {
                    el.TryAdd("isRequired", true);
                }
                if (attr.AttributeType == typeof(DisplayAttribute))
                {
                    var autoGenerated = getAttributeValueByProp(attr, "AutoGenerateField");
                    if (autoGenerated != null && (bool)autoGenerated == true) return null;
                    var title = getAttributeValueByProp(attr, "Name");
                    if (title != null)
                    {
                        el.TryAdd("title", title);
                    }
                }
                if(attr.AttributeType == typeof(FormFieldAttribute))
                {
                    this.setElementAttrByFormFieldAttr(el, attr);
                }
            }
            if (!this.hasProperty(el, "title"))
            {
                el.TryAdd("title", getTitleByName(prop.Name));
            }
            return el;
        }
        private void setElementAttrByPropType(ExpandoObject el, PropertyInfo prop)
        {
            if (this.isGenericListType(prop.PropertyType))
            {
                Type argType = prop.PropertyType.GetGenericArguments()[0];
                if (argType == typeof(string) || IsNumericType(argType)) {
                    el.TryAdd("type", "checkbox");
                } else
                {
                    this.setListElementAttrByPropType(el, prop);
                }
                return;
            }
            string type = prop.PropertyType == typeof(bool) ? "boolean" : "text";
            el.TryAdd("type", type);
            if(prop.PropertyType == typeof(DateTime))
            {
                el.TryAdd("inputType", "date");
            }
            if(IsNumericType(prop.PropertyType))
            {
                el.TryAdd("inputType", "number");
            }
        }
        private void setListElementAttrByPropType(ExpandoObject el, PropertyInfo prop)
        {
            el.TryAdd("type", "matrixdynamic");
            el.TryAdd("rowCount", 0);
            Type rowType = prop.PropertyType.GenericTypeArguments[0];
            PropertyInfo[] properties = rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var columns = new List<dynamic>();
            foreach(PropertyInfo colProp in properties)
            {
                dynamic column = this.createElementByProp(colProp);
                if(column != null)
                {
                    var dic = column as IDictionary<string, Object>;
                    dic["cellType"] = dic["type"];
                    dic.Remove("type");
                    columns.Add(column);
                }
            }
            el.TryAdd("columns", columns);
        }

        private bool isGenericListType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
        private void setElementAttrByFormFieldAttr(ExpandoObject el, CustomAttributeData attr)
        {
            var choicesByUrl = getAttributeValueByProp(attr, "ChoicesByUrl");
            var choices = getAttributeValueByProp(attr, "Choices");
            var fieldType = getAttributeValueByProp(attr, "FieldType");
            var inputType = getAttributeValueByProp(attr, "InputType");
            var dic = el as IDictionary<string, Object>;
            if (fieldType == null && (choicesByUrl != null || choices != null))
            {
                fieldType = "dropdown";
            }
            if (fieldType != null)
            {
                dic["type"] = fieldType;
            }
            if(choicesByUrl != null)
            {
                el.TryAdd("choicesByUrl", new {  url = choicesByUrl });
            }
            if(choices != null)
            {
                el.TryAdd("choices", convertToStringArray((ReadOnlyCollection<CustomAttributeTypedArgument>)choices));
            }
            if (inputType != null)
            {
                dic["inputType"] = inputType;
            }
        }
        private dynamic createNewPage(IList<dynamic> elements, CustomAttributeData pageAttr)
        {
            var res = new ExpandoObject();
            res.TryAdd("elements", elements);
            var title = this.getAttributeValueByProp(pageAttr, "Title");
            if(title != null)
            {
                res.TryAdd("title", title);
            }
            return res;
        }
        private CustomAttributeData getFormPageAttribute(PropertyInfo prop)
        {
            return prop.CustomAttributes.FirstOrDefault<CustomAttributeData>(item => item.AttributeType == typeof(FormPageAttribute));
        }
        private object getAttributeValueByProp(CustomAttributeData attr, string name)
        {
            if (attr == null) return null;
            var arg = attr.NamedArguments.FirstOrDefault<CustomAttributeNamedArgument>(item => item.MemberName == name);
            return arg != null ? arg.TypedValue.Value : null;
        }
        private bool hasProperty(ExpandoObject obj, string propName)
        {
            return ((IDictionary<String, object>)obj).ContainsKey(propName);
        }
        private string getTitleByName(string name)
        {
            string res = "";
            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]))
                {
                    res += ' ';
                }
                res += name[i];
            }
            return res;
        }
        private string[] convertToStringArray(ReadOnlyCollection<CustomAttributeTypedArgument> val)
        {
            var res = new List<string>();
            foreach(var obj in val)
            {
                res.Add(obj.Value.ToString());
            }
            return res.ToArray();
        }
        private static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
