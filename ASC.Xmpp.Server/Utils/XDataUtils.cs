/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2008 by AG-Software 											 *
 * All Rights Reserved.																 *
 * Contact information for AG-Software is available at http://www.ag-software.de	 *
 *																					 *
 * Licence:																			 *
 * The agsXMPP SDK is released under a dual licence									 *
 * agsXMPP can be used under either of two licences									 *
 * 																					 *
 * A commercial licence which is probably the most appropriate for commercial 		 *
 * corporate use and closed source projects. 										 *
 *																					 *
 * The GNU Public License (GPL) is probably most appropriate for inclusion in		 *
 * other open source projects.														 *
 *																					 *
 * See README.html for details.														 *
 *																					 *
 * For general enquiries visit our website at:										 *
 * http://www.ag-software.de														 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using XmppData = ASC.Xmpp.Core.protocol.x.data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASC.Xmpp.Server.Utils
{
    public class XDataFromsExample
	{
		[XDataUtils.XDataAnyOf("any", "all", "one of")]
		public string[] many { get; set; }

		[XDataUtils.XDataOneOf("any", "all", "one of")]
		public string oneof { get; set; }

		[XDataUtils.XDataFixed]
		public string data1 { get; set; }

		[XDataUtils.XDataMultiline]
		public string data2 { get; set; }

		[XDataUtils.XDataPassword]
		public string data3 { get; set; }

		[XDataUtils.XDataDescription("Label test")]
		public string data4 { get; set; }

		public XDataFromsExample()
		{
			many = new string[] { "any" };
			oneof = "all";
			data1 = "fixed!";
		}

	}



	public class XDataUtils
	{
		public class XDataDescriptionAttribute : Attribute
		{
			public string Description { get; set; }

			public XDataDescriptionAttribute(string description)
			{
				Description = description;
			}
		}

		public class XDataOneOfAttribute : Attribute
		{
			public string[] Variants { get; set; }

			public XDataOneOfAttribute(params string[] variants)
			{
				Variants = variants;
			}
		}

		public class XDataAnyOfAttribute : Attribute
		{
			public string[] Variants { get; set; }

			public XDataAnyOfAttribute(params string[] variants)
			{
				Variants = variants;
			}
		}

		public class XDataMultiline : Attribute
		{
		}

		public class XDataFixed : Attribute
		{
		}

		public class XDataPassword : Attribute
		{
		}

        public static void FillDataTo(object dataForm, string prefix, XmppData.Data data)
		{
            if (data.Type == XmppData.XDataFormType.submit)
			{
				//Gen prop map
				PropertyInfo[] props =
					dataForm.GetType().GetProperties(BindingFlags.Instance | BindingFlags.SetProperty |
													 BindingFlags.Public);
				Dictionary<string, PropertyInfo> propsVar = new Dictionary<string, PropertyInfo>();
				foreach (PropertyInfo prop in props)
				{
					if (prop.CanWrite)
					{
						propsVar.Add(string.Format("{0}#{1}", prefix, prop.Name), prop);
					}
				}

                XmppData.Field[] fields = data.GetFields();
				foreach (var field in fields)
				{
					if (propsVar.ContainsKey(field.Var))
					{
						PropertyInfo prop = propsVar[field.Var];
						if (prop.PropertyType == typeof(bool))
						{
							string val = field.GetValue();
							if (!string.IsNullOrEmpty(val))
							{
								prop.SetValue(dataForm, val == "1", null);
							}
						}
						else if (prop.PropertyType == typeof(string))
						{
							string val = field.GetValue();
							if (!string.IsNullOrEmpty(val))
							{
								prop.SetValue(dataForm, val, null);
							}
						}
						else if (prop.PropertyType == typeof(string[]))
						{
							string[] val = field.GetValues();
							if (val != null)
							{
								prop.SetValue(dataForm, val, null);
							}
						}

					}
				}
			}
		}

        public static XmppData.Data GetDataForm(object dataForm, string prefix)
		{
            XmppData.Data data = new XmppData.Data(XmppData.XDataFormType.form);

			//Go through public vars
			PropertyInfo[] props = dataForm.GetType().GetProperties(BindingFlags.Instance | BindingFlags.SetProperty |
											 BindingFlags.Public);
			foreach (PropertyInfo prop in props)
			{
				if (prop.CanRead)
				{
                    XmppData.Field field = new XmppData.Field(XmppData.FieldType.Unknown);

					field.Var = string.Format("{0}#{1}", prefix, prop.Name);
					object propValue = prop.GetValue(dataForm, null);

					foreach (var attribute in prop.GetCustomAttributes(false))
					{
						if (attribute is XDataDescriptionAttribute)
						{
							field.Label = (attribute as XDataDescriptionAttribute).Description;
						}
						else if (attribute is XDataOneOfAttribute)
						{
                            field.Type = XmppData.FieldType.List_Single;
							field.FieldValue = (string)propValue;
							foreach (var vars in (attribute as XDataOneOfAttribute).Variants)
							{
								field.AddOption(vars, vars);
							}
						}
						else if (attribute is XDataAnyOfAttribute)
						{
                            field.Type = XmppData.FieldType.List_Multi;
							field.AddValues((string[])propValue);
							foreach (var vars in (attribute as XDataAnyOfAttribute).Variants)
							{
								field.AddOption(vars, vars);
							}
						}
						else if (attribute is XDataMultiline)
						{
                            field.Type = XmppData.FieldType.Text_Multi;
							field.FieldValue = (string)propValue;
						}
						else if (attribute is XDataPassword)
						{
                            field.Type = XmppData.FieldType.Text_Private;
							field.FieldValue = (string)propValue;
						}
						else if (attribute is XDataFixed)
						{
                            field.Type = XmppData.FieldType.Fixed;
							field.FieldValue = (string)propValue;
						}
					}
                    if (field.Type == XmppData.FieldType.Unknown)
					{
						if (prop.PropertyType == typeof(bool))
						{
                            field.Type = XmppData.FieldType.Boolean;
							field.FieldValue = (bool)propValue ? "1" : "0";
						}
						else if (prop.PropertyType == typeof(string))
						{
                            field.Type = XmppData.FieldType.Text_Single;
							field.FieldValue = (string)propValue;
						}
					}
					if (field.Label == null)
					{
						field.Label = prop.Name;
					}
					data.AddField(field);
				}
			}

			return data;
		}
	}
}