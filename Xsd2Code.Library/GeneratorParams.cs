﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Xsd2Code.Library.Helpers;

namespace Xsd2Code.Library
{
    public class GeneratorParamsBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    [Serializable]
    public class MiscellaneousParams : GeneratorParamsBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether [disable debug].
        /// </summary>
        /// <value><c>true</c> if [disable debug]; otherwise, <c>false</c>.</value>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicating whether if generate attribute for debug into generated code.")]
        public bool DisableDebug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if generate EditorBrowsableState.Never attribute
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicating whether if generate EditorBrowsableState.Never attribute.")]
        public bool HidePrivateFieldInIde { get; set; }

        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicating to exclude class generation types includes/imported into schema.")]
        public bool ExcludeIncludedTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if generate summary documentation
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicating whether if generate summary documentation from XSD annotation.")]
        public bool EnableSummaryComment { get; set; }
    }

    [Serializable]
    public class PropertyParams : GeneratorParamsBase
    {
        private readonly GeneratorParams mainParamsFields;

        /// <summary>
        /// Indicate if use automatic properties
        /// </summary>
        private bool automaticPropertiesField;

        public PropertyParams(GeneratorParams mainParams)
        {
            mainParamsFields = mainParams;
        }

        /// <summary>
        /// Gets or sets a value indicating whether if implement INotifyPropertyChanged
        /// </summary>
        [Category("Property")]
        [DefaultValue(false)]
        [Description("Use lazy pattern when possible.")]
        public bool EnableLazyLoading { get; set; }

        [Category("Property")]
        [DefaultValue(false)]
        [Description("Enable/Disable virtual properties. Use with NHibernate.")]
        public bool EnableVirtualProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether serialize/deserialize method support
        /// </summary>
        [Category("Property")]
        [DefaultValue(false)]
        [Description("Generate automatic properties when possible. (Work only for csharp with target framework 3.0 or 3.5 and EnableDataBinding disable)")]
        public bool AutomaticProperties
        {
            get
            {
                return this.automaticPropertiesField;
            }

            set
            {
                if (value)
                {
                    if (this.mainParamsFields.TargetFramework != TargetFramework.Net20)
                    {
                        this.automaticPropertiesField = true;
                        this.mainParamsFields.EnableDataBinding = false;
                    }
                }
                else
                {
                    this.automaticPropertiesField = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if generate EditorBrowsableState.Never attribute
        /// </summary>
        [Category("Property")]
        [DefaultValue(false)]
        [Description("ShouldSerializeProperty is only useful for nullable type. If nullable type has no value,  the XMLSerialiser will skip the property.")]
        public bool GenerateShouldSerializeProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if generate EditorBrowsableState.Never attribute
        /// </summary>
        [Category("Property")]
        [DefaultValue(false)]
        [Description("[PropertyName]Specified property indicates whether the property should be ignored by the XMLSerialiser.")]
        public PropertyNameSpecifiedType GeneratePropertyNameSpecified { get; set; }
    }

    [Serializable]
    public class SerializeParams : GeneratorParamsBase
    {
        /// <summary>
        /// Gets or sets a value indicating the name of Serialize method.
        /// </summary>
        [Category("Serialize"), DefaultValue("Serialize"), Description("The name of Serialize method.")]
        public string SerializeMethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of Deserialize method.
        /// </summary>
        [Category("Serialize"), DefaultValue("Deserialize"), Description("The name of deserialize method.")]
        public string DeserializeMethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of Serialize method.
        /// </summary>
        [Category("Serialize"), DefaultValue("SaveToFile"), Description("The name of save to XML file method.")]
        public string SaveToFileMethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of SaveToFile method.
        /// </summary>
        [Category("Serialize"), DefaultValue("LoadFromFile"), Description("The name of load from XML file method.")]
        public string LoadFromFileMethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether serialize/deserialize method support
        /// </summary>
        [Category("Serialize")]
        [DefaultValue(false)]
        [Description("Indicating whether serialize/deserialize method must be generate.")]
        public bool Enabled { get; set; }

        public override string ToString()
        {
            return string.Format("(Serialize methods={0})", Enabled);
        }

        /// <summary>
        /// Gets or sets a value indicating whether support  .
        /// </summary>
        [Category("Serialize")]
        [DefaultValue(false)]
        [Description("Enable/Disable text encoding.")]
        public bool EnableEncoding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating default encoder for serialize/deserialize
        /// </summary>
        [Category("Serialize")]
        [DefaultValue(DefaultEncoder.UTF8)]
        [Description("Specifies the default encoding for XML Serialization (ASCII, UNICODE, UTF8...).")]
        public DefaultEncoder DefaultEncoder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if generate EditorBrowsableState.Never attribute
        /// </summary>
        [Category("Serialize")]
        [DefaultValue(false)]
        [Description("Indicating whether if generate .NET 2.0 serialization attributes. If false, serialization will use propertyName")]
        public bool GenerateXmlAttributes { get; set; }
        private bool _generateOrderXmlAttributes;

        /// <summary>
        /// Gets or sets a value indicating the name of SaveToFile method.
        /// </summary>
        [Category("Serialize"), DefaultValue(false),
         Description("Generate order XML Attribute (Work only if GenerateXmlAttributes is true).")]
        public bool GenerateOrderXmlAttributes
        {
            get { return _generateOrderXmlAttributes; }
            set
            {
                _generateOrderXmlAttributes = value;
                OnPropertyChanged("GenerateOrderXmlAttributes");
            }
        }

        public string GetEncoderString()
        {
            switch (DefaultEncoder)
            {
                case DefaultEncoder.ASCII:
                    return "Encoding.ASCII";
                case DefaultEncoder.Unicode:
                    return "Encoding.Unicode";
                case DefaultEncoder.BigEndianUnicode:
                    return "Encoding.BigEndianUnicode";
                case DefaultEncoder.UTF32:
                    return "Encoding.UTF32";
                case DefaultEncoder.Default:
                    return "Encoding.Default";
            }
            return "Encoding.UTF8";
        }
    }


    [Serializable]
    public class TrackingChangesParams : GeneratorParamsBase
    {
        private bool enabledField;

        [DefaultValue(false)]
        public bool Enabled
        {
            get { return enabledField; }
            set
            {
                if (!enabledField.Equals(value))
                {
                    enabledField = value;
                    OnPropertyChanged("Enabled");
                }
            }
        }

        [DefaultValue(true), Description("If true, generate tracking changes classes inside [SchemaName].designed.cs file.")]
        public bool GenerateTrackingClasses { get; set; }

        public override string ToString()
        {
            return string.Format("(Tracking changes={0})", enabledField);
        }
    }

    public class GenericBaseClassParams : GeneratorParamsBase
    {
        private bool enabledField;

        [Category("Behavior"), DefaultValue(false), Description("Use generic partial base class for all methods")]
        public bool Enabled
        {
            get { return enabledField; }
            set
            {
                if (!enabledField.Equals(value))
                {
                    enabledField = value;
                    OnPropertyChanged("Enabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the name of Serialize method.
        /// </summary>
        [DefaultValue("EntityBase"), Description("Name of generic partial base class")]
        public string BaseClassName { get; set; }

        /// <summary>
        /// Generate a base class
        /// </summary>
        [DefaultValue("true"), Description("Generate base class code inside the output file")]
        public bool GenerateBaseClass { get; set; }

        public override string ToString()
        {
            return string.Format("(Use generic base class={0})", enabledField);
        }
    }
    /// <summary>
    /// Represents all generation parameters
    /// </summary>
    /// <remarks>
    /// Developer Notes:
    /// When adding a new parameter:
    /// 
    ///     Add the Property
    ///     Add it to ToXmlTag() (if for saving/loading)
    ///     Add it to LoadFromFile(string xsdFilePath, out string outputFile) (if for saving/loading)
    ///     Add The Tag Constant to GeneratorContext
    ///     Add it to the CommandLine Options Project Xsd2Code->EntryPoint.Main
    /// 
    /// Revision history:
    /// 
    ///     Modified 2009-02-20 by Ruslan Urban
    ///     Added TargetFramework and GenerateCloneMethod properties
    ///     Modified 2009-05-18 by Pascal Cabanel.
    ///     Added NET 2.0 serialization attributes as an option
    ///     Added The ability to create a default param: defaults.xsd2code,
    ///     
    /// </remarks>
    public class GeneratorParams
    {
        #region Private

        /// <summary>
        /// Type of collection
        /// </summary>
        private CollectionType collectionObjectTypeField = CollectionType.List;

        /// <summary>
        /// List of custom usings
        /// </summary>
        private List<NamespaceParam> customUsingsField = new List<NamespaceParam>();

        /// <summary>
        /// Indicate if use generic base class for isolate all methods
        /// </summary>
        private GenericBaseClassParams genericBaseClassField;

        /// <summary>
        /// Indicate if use tracking change algorithm.
        /// </summary>
        private TrackingChangesParams trackingChangesField;

        /// <summary>
        /// Serialisation params
        /// </summary>
        private SerializeParams serializeFiledField;

        /// <summary>
        /// Miscellaneous properties
        /// </summary>
        private MiscellaneousParams miscellaneousParamsField;

        /// <summary>
        /// Configure options properties
        /// </summary>
        private bool enableDataBindingField;

        /// <summary>
        /// Indicate if implement INotifyPropertyChanged
        /// </summary>
        private PropertyParams propertyParamsField;

        /// <summary>
        /// Indicate the target framework
        /// </summary>
        private TargetFramework targetFrameworkField = default(TargetFramework);

        /// <summary>
        /// Indicate the output language
        /// </summary>
        private GenerationLanguage language;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorParams"/> class.
        /// </summary>
        public GeneratorParams(string inputXsdFile = "")
        {
            this.Serialization.LoadFromFileMethodName = "LoadFromFile";
            this.Serialization.SaveToFileMethodName = "SaveToFile";
            this.Serialization.DeserializeMethodName = "Deserialize";
            this.Serialization.SerializeMethodName = "Serialize";
            this.GenericBaseClass.BaseClassName = "EntityBase";
            this.GenericBaseClass.Enabled = false;
            this.EnableInitializeFields = true;
            this.Miscellaneous.ExcludeIncludedTypes = false;
            this.TrackingChanges.PropertyChanged += TrackingChangesPropertyChanged;
            this.Serialization.DefaultEncoder = DefaultEncoder.UTF8;
            this.GenerateSeparateFiles = false;
            this.OutputFilePath = string.IsNullOrEmpty(inputXsdFile) ? "" : Path.ChangeExtension(inputXsdFile, ".designer.cs");
            this.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;
            this.InputXsdString = "";
        }

        /// <summary>
        /// Tracking the changes property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void TrackingChangesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Enabled")
            {
                if (this.TrackingChanges.Enabled)
                {
                    this.EnableDataBinding = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name space.
        /// </summary>
        /// <value>The name space.</value>
        [Category("Code")]
        [Description("namespace of generated file")]
        public string NameSpace { get; set; }

        /// <summary>
        /// Gets or sets generation language
        /// </summary>
        [Category("Code")]
        [Description("Language")]
        public GenerationLanguage Language
        {
            get
            {
                return language;
            }
            set
            {
                language = value;
                if (!string.IsNullOrEmpty(OutputFilePath))
                {
                    if (value == GenerationLanguage.CSharp)
                        OutputFilePath = Path.ChangeExtension(OutputFilePath, ".cs");
                    if (value == GenerationLanguage.VisualBasic)
                        OutputFilePath = Path.ChangeExtension(OutputFilePath, ".vb");
                }
            }
        }

        /// <summary>
        /// Gets or sets the output file path.
        /// </summary>
        /// <value>The output file path.</value>
        [Category("Code")]
        public string OutputFilePath { get; set; }

        /// <summary>
        /// Gets or sets the input file path.
        /// </summary>
        /// <value>The input file path.</value>
        [Browsable(false)]
        public string InputFilePath { get; set; }
        
        
        [Description("XSD string. Used when no InputFilePath specified.")]
        public string InputXsdString { get; set; }
        

        /// <summary>
        /// Gets or sets collection type to use for code generation
        /// </summary>
        [Category("Collection")]
        [Description("Set type of collection for unbounded elements")]
        public CollectionType CollectionObjectType
        {
            get { return this.collectionObjectTypeField; }
            set { this.collectionObjectTypeField = value; }
        }

        /// <summary>
        /// Gets or sets collection base
        /// </summary>
        [Category("Collection")]
        [Description("Set the collection base if using CustomCollection")]
        public string CollectionBase { get; set; }

        /// <summary>
        /// Gets or sets custom usings
        /// </summary>
        [Category("Code")]
        [Description("list of custom using for CustomCollection")]
        public List<NamespaceParam> CustomUsings
        {
            get { return this.customUsingsField; }
            set { this.customUsingsField = value; }
        }

        /// <summary>
        /// Gets or sets the custom usings string.
        /// </summary>
        /// <value>The custom usings string.</value>
        [Browsable(false)]
        public string CustomUsingsString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if implement INotifyPropertyChanged
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicating whether if implement INotifyPropertyChanged.")]
        public bool EnableDataBinding
        {
            get
            {
                return this.enableDataBindingField;
            }

            set
            {
                this.enableDataBindingField = value;
                if (this.enableDataBindingField)
                {
                    this.PropertyParams.AutomaticProperties = false;
                }
                else
                {
                    this.TrackingChanges.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether serialize/deserialize method support
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Generate clone method based on MemberwiseClone")]
        public bool GenerateCloneMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether serialize/deserialize method support
        /// </summary>
        [Category("Code")]
        [DefaultValue(Library.TargetFramework.Net20)]
        [Description("Generated code base")]
        public TargetFramework TargetFramework
        {
            get
            {
                return this.targetFrameworkField;
            }

            set
            {
                this.targetFrameworkField = value;
                if (this.targetFrameworkField == TargetFramework.Net20)
                {
                    this.PropertyParams.AutomaticProperties = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if generate summary documentation
        /// </summary>
        [Category("Behavior")]
        [Description("Track changes.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Browsable(false)]
        public TrackingChangesParams TrackingChanges
        {
            get
            {
                if (trackingChangesField == null)
                {
                    trackingChangesField = new TrackingChangesParams();
                }
                return trackingChangesField;
            }

            set
            {
                trackingChangesField = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if generate summary documentation
        /// </summary>
        [Category("Behavior")]
        [Description("Configure options properties.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PropertyParams PropertyParams
        {
            get
            {
                if (propertyParamsField == null)
                {
                    propertyParamsField = new PropertyParams(this);
                }
                return propertyParamsField;
            }

            set
            {
                propertyParamsField = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether if generate summary documentation
        /// </summary>
        [Category("Behavior")]
        [Description("XML Serialization configuration.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SerializeParams Serialization
        {
            get
            {
                if (serializeFiledField == null)
                {
                    serializeFiledField = new SerializeParams();
                }
                return serializeFiledField;
            }

            set
            {
                serializeFiledField = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if generate summary documentation
        /// </summary>
        [Category("Behavior")]
        [Description("Miscellaneous")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MiscellaneousParams Miscellaneous
        {
            get
            {
                if (miscellaneousParamsField == null)
                {
                    miscellaneousParamsField = new MiscellaneousParams();
                }
                return miscellaneousParamsField;
            }

            set
            {
                miscellaneousParamsField = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if generate summary documentation
        /// </summary>
        [Category("Behavior")]
        [Description("Generic base class configuration.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericBaseClassParams GenericBaseClass
        {
            get
            {
                if (genericBaseClassField == null)
                {
                    genericBaseClassField = new GenericBaseClassParams();
                }
                return genericBaseClassField;
            }

            set
            {
                genericBaseClassField = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if generate summary documentation
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Generate WCF data contract attributes")]
        public bool GenerateDataContracts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether accessing a property will initialize it
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Enable/Disable Global initialization of the fields in both Constructors, Lazy Properties. Maximum override")]
        public bool EnableInitializeFields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to generate the code in one file or one file per type
        /// </summary>
        [Category("Code")]
        [DefaultValue(false)]
        [Description("Generated in separate files")]
        public bool GenerateSeparateFiles
        {
            get; set;
        }

        [Category("Code")]
        [DefaultValue(1)]
        [Description("Specifies various options to use when generating .NET types")]
        public CodeGenerationOptions CodeGenerationOptions { get; set; }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="xsdFilePath">The XSD file path.</param>
        /// <returns>GeneratorParams instance</returns>
        public static GeneratorParams LoadFromFile(string xsdFilePath)
        {
            string outFile;
            return LoadFromFile(xsdFilePath, out outFile);
        }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="xsdFilePath">The XSD file path.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="previousOutputFile">An optional previous output file (useful to read the previous parameters)</param>
        /// <returns>GeneratorParams instance</returns>

        public static GeneratorParams LoadFromFile(string xsdFilePath, out string outputFile, string previousOutputFile = null)
        {
            var parameters = new GeneratorParams();


            #region Search generationFile
            outputFile = string.Empty;

            var localDir = Path.GetDirectoryName(xsdFilePath);

            var schemaConfigPath = Path.ChangeExtension(xsdFilePath, "xsd.xsd2code");

            var defaultsConfigPath = Path.Combine(localDir, "defaults.xsd2code");

            var configFile = string.Empty;

            //Try local <schemaName>.xsd.xsd2code if exist Use this file always.
            if (File.Exists(schemaConfigPath))
            {
                configFile = schemaConfigPath;
                //outputFile = Utility.GetOutputFilePath(xsdFilePath, language);
            }
            else
            {
                //Load from the output files if one of them exist
                foreach (GenerationLanguage language in Enum.GetValues(typeof(GenerationLanguage)))
                {
                    if (!string.IsNullOrEmpty(previousOutputFile))
                    {
                        if (File.Exists(previousOutputFile))
                        {
                            configFile = previousOutputFile;
                            break;
                        }
                    }
                    string fileName = Utility.GetOutputFilePath(xsdFilePath, language);
                    if (File.Exists(fileName))
                    {
                        configFile = fileName;
                        break;
                    }
                }
                //If there isn't a Schema Config File or an output File see if there is a defaults file
                if (outputFile.Length == 0)
                {
                    if (File.Exists(defaultsConfigPath))
                    {
                        configFile = defaultsConfigPath;
                    }
                }
            }


            if (configFile.Length == 0)
                return null;

            #endregion

            #region Try to get Last options

            //DCM Created Routine to Search for Auto-Generated Parameters
            var optionLine = ExtractAutoGeneratedParams(configFile);

            //DCM Fall back to old method because of some invalid Tag names
            if (optionLine == null)
            {
                using (TextReader streamReader = new StreamReader(configFile))
                {
                    streamReader.ReadLine();
                    streamReader.ReadLine();
                    streamReader.ReadLine();
                    optionLine = streamReader.ReadLine();
                }
            }


            if (optionLine != null)
            {
                parameters.NameSpace = optionLine.ExtractStrFromXML(GeneratorContext.NAMESPACETAG);
                parameters.CollectionObjectType = Utility.ToEnum<CollectionType>(optionLine.ExtractStrFromXML(GeneratorContext.COLLECTIONTAG));
                parameters.Language = Utility.ToEnum<GenerationLanguage>(optionLine.ExtractStrFromXML(GeneratorContext.CODETYPETAG));
                parameters.EnableDataBinding = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.ENABLEDATABINDINGTAG));
                parameters.GenerateSeparateFiles = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.GENERATESEPARATEFILES));
                parameters.OutputFilePath = optionLine.ExtractStrFromXML(GeneratorContext.OUTPUTFILEPATH);
                parameters.PropertyParams.EnableLazyLoading = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.ENABLELAZYLOADINGTAG));
                parameters.Miscellaneous.HidePrivateFieldInIde = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.HIDEPRIVATEFIELDTAG));
                parameters.Miscellaneous.EnableSummaryComment = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.ENABLESUMMARYCOMMENTTAG));
                parameters.TrackingChanges.Enabled = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.ENABLETRACKINGCHANGESTAG));
                parameters.TrackingChanges.GenerateTrackingClasses = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.GENERATETRACKINGCLASSESTAG));
                parameters.Serialization.Enabled = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.INCLUDESERIALIZEMETHODTAG));
                parameters.Serialization.EnableEncoding = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.ENABLEENCODINGTAG));
                parameters.Serialization.DefaultEncoder = Utility.ToEnum<DefaultEncoder>(optionLine.ExtractStrFromXML(GeneratorContext.DEFAULTENCODERTAG));
                parameters.GenerateCloneMethod = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.GENERATECLONEMETHODTAG));
                parameters.GenerateDataContracts = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.GENERATEDATACONTRACTSTAG));
                parameters.TargetFramework = Utility.ToEnum<TargetFramework>(optionLine.ExtractStrFromXML(GeneratorContext.CODEBASETAG));
                parameters.Miscellaneous.DisableDebug = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.DISABLEDEBUGTAG));
                parameters.Serialization.GenerateXmlAttributes = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.GENERATEXMLATTRIBUTESTAG));
                parameters.Serialization.GenerateOrderXmlAttributes = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.ORDERXMLATTRIBUTETAG));
                parameters.PropertyParams.AutomaticProperties = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.AUTOMATICPROPERTIESTAG));
                parameters.PropertyParams.EnableVirtualProperties = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.ENABLEVIRTUALPROPERTIESTAG));
                parameters.GenericBaseClass.Enabled = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.USEGENERICBASECLASSTAG));
                parameters.GenericBaseClass.GenerateBaseClass = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.GENERATEBASECLASSTAG));
                parameters.PropertyParams.GenerateShouldSerializeProperty = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.GENERATESHOULDSERIALIZETAG));
                parameters.EnableInitializeFields = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.ENABLEINITIALIZEFIELDSTAG), true);
                parameters.Miscellaneous.ExcludeIncludedTypes = Utility.ToBoolean(optionLine.ExtractStrFromXML(GeneratorContext.EXCLUDEINCLUDEDTYPESTAG));
                parameters.PropertyParams.GeneratePropertyNameSpecified = Utility.ToEnum<PropertyNameSpecifiedType>(optionLine.ExtractStrFromXML(GeneratorContext.GENERATEPROPERTYNAMESPECIFIEDTAG));

                string str = optionLine.ExtractStrFromXML(GeneratorContext.SERIALIZEMETHODNAMETAG);
                parameters.Serialization.SerializeMethodName = str.Length > 0 ? str : "Serialize";

                str = optionLine.ExtractStrFromXML(GeneratorContext.DESERIALIZEMETHODNAMETAG);
                parameters.Serialization.DeserializeMethodName = str.Length > 0 ? str : "Deserialize";

                str = optionLine.ExtractStrFromXML(GeneratorContext.SAVETOFILEMETHODNAMETAG);
                parameters.Serialization.SaveToFileMethodName = str.Length > 0 ? str : "SaveToFile";

                str = optionLine.ExtractStrFromXML(GeneratorContext.LOADFROMFILEMETHODNAMETAG);
                parameters.Serialization.LoadFromFileMethodName = str.Length > 0 ? str : "LoadFromFile";

                str = optionLine.ExtractStrFromXML(GeneratorContext.BASECLASSNAMETAG);
                parameters.GenericBaseClass.BaseClassName = str.Length > 0 ? str : "EntityBase";

                // TODO:get custom using
                string customUsingString = optionLine.ExtractStrFromXML(GeneratorContext.CUSTOMUSINGSTAG);
                if (!string.IsNullOrEmpty(customUsingString))
                {
                    string[] usings = customUsingString.Split(';');
                    foreach (string item in usings)
                        parameters.CustomUsings.Add(new NamespaceParam { NameSpace = item });
                }
                parameters.CollectionBase = optionLine.ExtractStrFromXML(GeneratorContext.COLLECTIONBASETAG);
            }

            return parameters;

            #endregion
        }

        /// <summary>
        /// Gets the params.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>GeneratorParams instance</returns>
        public static GeneratorParams GetParams(string parameters)
        {
            var newparams = new GeneratorParams();

            return newparams;
        }

        /// <summary>
        /// Save values into XML string
        /// </summary>
        /// <returns>XML string value</returns>
        public string ToXmlTag()
        {
            // order does not matter, but would be nice to have same order as in parsing method 'LoadFromFile'
            var tagsAndValues = new Dictionary<string, object> {
                { GeneratorContext.NAMESPACETAG, this.NameSpace },
                { GeneratorContext.COLLECTIONTAG, this.CollectionObjectType },
                { GeneratorContext.COLLECTIONBASETAG, this.CollectionBase },
                { GeneratorContext.CODETYPETAG, this.Language },
                { GeneratorContext.GENERATESEPARATEFILES, this.GenerateSeparateFiles },
                { GeneratorContext.OUTPUTFILEPATH, this.OutputFilePath },
                { GeneratorContext.ENABLEDATABINDINGTAG, this.EnableDataBinding },
                { GeneratorContext.ENABLELAZYLOADINGTAG, this.PropertyParams.EnableLazyLoading },
                { GeneratorContext.HIDEPRIVATEFIELDTAG, this.Miscellaneous.HidePrivateFieldInIde },
                { GeneratorContext.ENABLESUMMARYCOMMENTTAG, this.Miscellaneous.EnableSummaryComment },
                { GeneratorContext.ENABLETRACKINGCHANGESTAG, this.TrackingChanges.Enabled },
                { GeneratorContext.GENERATETRACKINGCLASSESTAG, this.TrackingChanges.GenerateTrackingClasses },
                { GeneratorContext.ENABLEVIRTUALPROPERTIESTAG, this.PropertyParams.EnableVirtualProperties },
                { GeneratorContext.INCLUDESERIALIZEMETHODTAG, this.Serialization.Enabled },
                { GeneratorContext.ENABLEENCODINGTAG, this.Serialization.EnableEncoding },
                { GeneratorContext.DEFAULTENCODERTAG, this.Serialization.DefaultEncoder },
                { GeneratorContext.USEGENERICBASECLASSTAG, this.GenericBaseClass.Enabled },
                { GeneratorContext.BASECLASSNAMETAG, this.GenericBaseClass.BaseClassName },
                { GeneratorContext.GENERATEBASECLASSTAG, this.GenericBaseClass.GenerateBaseClass },
                { GeneratorContext.GENERATECLONEMETHODTAG, this.GenerateCloneMethod },
                { GeneratorContext.GENERATEDATACONTRACTSTAG, this.GenerateDataContracts },
                { GeneratorContext.CODEBASETAG, this.TargetFramework },
                { GeneratorContext.SERIALIZEMETHODNAMETAG, this.Serialization.SerializeMethodName },
                { GeneratorContext.DESERIALIZEMETHODNAMETAG, this.Serialization.DeserializeMethodName },
                { GeneratorContext.SAVETOFILEMETHODNAMETAG, this.Serialization.SaveToFileMethodName },
                { GeneratorContext.LOADFROMFILEMETHODNAMETAG, this.Serialization.LoadFromFileMethodName },
                { GeneratorContext.GENERATEXMLATTRIBUTESTAG, this.Serialization.GenerateXmlAttributes },
                { GeneratorContext.ORDERXMLATTRIBUTETAG, this.Serialization.GenerateOrderXmlAttributes },
                { GeneratorContext.AUTOMATICPROPERTIESTAG, this.PropertyParams.AutomaticProperties },
                { GeneratorContext.GENERATESHOULDSERIALIZETAG, this.PropertyParams.GenerateShouldSerializeProperty },
                { GeneratorContext.DISABLEDEBUGTAG, this.Miscellaneous.DisableDebug },
                { GeneratorContext.GENERATEPROPERTYNAMESPECIFIEDTAG, this.PropertyParams.GeneratePropertyNameSpecified },
                { GeneratorContext.EXCLUDEINCLUDEDTYPESTAG, this.Miscellaneous.ExcludeIncludedTypes },
                { GeneratorContext.ENABLEINITIALIZEFIELDSTAG, this.EnableInitializeFields }
            };
            if (this.CustomUsings != null)
            {
                tagsAndValues.Add(GeneratorContext.CUSTOMUSINGSTAG, string.Join(";",
                    this.CustomUsings.Select(x => x.NameSpace).Where(usingNamespace => !string.IsNullOrEmpty(usingNamespace)).ToArray()));
            }

            var optionsLine = new StringBuilder();
            foreach (var pair in tagsAndValues)
            {
                var stringVal = Convert.ToString(pair.Value);
                if (!string.IsNullOrEmpty(stringVal))
                {
                    optionsLine.Append(XmlHelper.InsertXMLFromStr(pair.Key, stringVal));
                }
            }
            return optionsLine.ToString();
        }

        /// <summary>
        /// Shallow clone
        /// </summary>
        /// <returns></returns>
        public GeneratorParams Clone()
        {
            return MemberwiseClone() as GeneratorParams;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public Result Validate()
        {
            var result = new Result(true);

            #region Validate input

            if (string.IsNullOrEmpty(this.NameSpace))
            {
                result.Success = false; result.Messages.Add(MessageType.Error, "you must specify the NameSpace");
            }

            if (this.CollectionObjectType.ToString() == CollectionType.DefinedType.ToString())
            {
                if (string.IsNullOrEmpty(this.CollectionBase))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, "you must specify the custom collection base type");
                }
            }

            if (this.Serialization.Enabled)
            {
                if (string.IsNullOrEmpty(this.Serialization.SerializeMethodName))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, "you must specify the Serialize method name.");
                }

                if (!IsValidMethodName(this.Serialization.SerializeMethodName))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, string.Format("Serialize method name {0} is invalid.",
                                                  this.Serialization.SerializeMethodName));
                }

                if (string.IsNullOrEmpty(this.Serialization.DeserializeMethodName))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, "you must specify the Deserialize method name.");
                }

                if (!IsValidMethodName(this.Serialization.DeserializeMethodName))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, string.Format("Deserialize method name {0} is invalid.",
                                                  this.Serialization.DeserializeMethodName));
                }

                if (string.IsNullOrEmpty(this.Serialization.SaveToFileMethodName))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, "you must specify the save to XML file method name.");
                }

                if (!IsValidMethodName(this.Serialization.SaveToFileMethodName))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, string.Format("Save to file method name {0} is invalid.",
                                                  this.Serialization.SaveToFileMethodName));
                }

                if (string.IsNullOrEmpty(this.Serialization.LoadFromFileMethodName))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, "you must specify the load from XML file method name.");
                }

                if (!IsValidMethodName(this.Serialization.LoadFromFileMethodName))
                {
                    result.Success = false; result.Messages.Add(MessageType.Error, string.Format("Load from file method name {0} is invalid.",
                                                  this.Serialization.LoadFromFileMethodName));
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Validates the input.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static bool IsValidMethodName(string value)
        {
            foreach (char item in value)
            {
                int ascii = Convert.ToInt16(item);
                if ((ascii < 65 || ascii > 90) && (ascii < 97 || ascii > 122) && (ascii != 8))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Extracts the auto generated params from a File.
        /// Method doesn't rely on the position with in the file.
        /// extracts the values contained in the GeneratorContext.AUTOGENERATEDTAG
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>String containing the Pseudo XML tags for Generator Params</returns>
        /// <remarks>Doesn't rely on Position with in the file</remarks>
        private static string ExtractAutoGeneratedParams(string filePath)
        {

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("File containing Generator Parameters was not found {0}", filePath), filePath);
            }

            var options = new StringBuilder();
            using (var r = new StreamReader(filePath))
            {
                // Loop over each line in file
                bool appendLine = false;

                // Store line contents in this String.
                string line = null;

                do
                {
                    // Read in the next line.
                    line = r.ReadLine();

                    Console.WriteLine(line);

                    //skip the Empty lines
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    //Start appending
                    if (line.ToUpper().Contains("<" + GeneratorContext.AUTOGENERATEDTAG.ToUpper() + ">"))
                    {
                        appendLine = true;
                    }

                    //No matter what Append this line
                    if (appendLine == true)
                    {
                        options.Append(line).AppendLine();
                    }

                    //if this line contains the Closing tag exit
                    if (line.ToUpper().Contains("</" + GeneratorContext.AUTOGENERATEDTAG.ToUpper() + ">"))
                    {
                        break;
                    }
                }

                while (!r.EndOfStream);
            }
            return options.ToString();
        }


    }
}