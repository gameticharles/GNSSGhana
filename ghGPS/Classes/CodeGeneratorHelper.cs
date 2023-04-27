using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using Microsoft.CSharp;

namespace ghGPS.Classes
{
    public static class CodeGeneratorHelper
    {
        /// <summary>
        /// Creates a dynamic class in memory from a hashtable. Each entry in the hashtable will become a field
        /// with a corresponding property. The class will implement INotifyPropertyChanged for each of the
        /// properties.
        /// </summary>
        /// <param name="namespaceName">The name of the namespace that will contain the dynamic class.</param>
        /// <param name="className">The name of the dynamic class.</param>
        /// <param name="source">The source hashtable that the members of the dynamic class will based off of.</param>
        /// <param name="useKeysForNaming">If set to True, the keys of the hashtable will be converted to strings
        /// and the field names and property names of the dynamic class will be based off of those strings. If
        /// set to false, the fields and properties will be named in sequential order.</param>
        /// <returns>Returns the C# code as a string.</returns>
        public static string CreateClassFromHashtable(string namespaceName, string className, Hashtable source,  bool useKeysForNaming)
        {
            // Create compile unit.
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // Create namespace.
            CodeNamespace dynamicNamespace = new CodeNamespace(namespaceName);
            dynamicNamespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));

            // Create class.
            CodeTypeDeclaration dynamicClass = new CodeTypeDeclaration(className);
            dynamicClass.IsClass = true;
            dynamicClass.BaseTypes.Add(new CodeTypeReference("System.ComponentModel.INotifyPropertyChanged"));

            // Create PropertyChanged event; implement INotifyPropertyChanged.
            CodeMemberEvent propertyChangedEvent = new CodeMemberEvent();
            propertyChangedEvent.Name = "PropertyChanged";
            propertyChangedEvent.Type = new CodeTypeReference("System.ComponentModel.PropertyChangedEventHandler");
            propertyChangedEvent.Attributes = MemberAttributes.Public;
            dynamicClass.Members.Add(propertyChangedEvent);

            foreach (object key in source.Keys)
            {
                // Construct field and property names.
                string fieldName = string.Format("_{0}", key.ToString());
                string propertyName = key.ToString();

                // Create field.
                CodeMemberField dynamicField = new CodeMemberField(source[key].GetType(), fieldName);
                dynamicField.InitExpression = new CodePrimitiveExpression(source[key]);
                dynamicClass.Members.Add(dynamicField);

                // Create property.
                CodeMemberProperty dynamicProperty = new CodeMemberProperty();
                dynamicProperty.Name = key.ToString();
                dynamicProperty.Type = new CodeTypeReference(source[key].GetType());

                // Create property - get statements.
                dynamicProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));

                // Create property - set statements.
                // Assign value to field.
                dynamicProperty.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));

                // Call PropertyChanged event.
                // Create target object reference.
                CodeEventReferenceExpression propertyChangedTargetObject = new CodeEventReferenceExpression(
                    new CodeThisReferenceExpression(), "PropertyChanged");

                // Create parameters array.
                CodeExpression[] propertyChangedParameters = new CodeExpression[]
                {
                    new CodeThisReferenceExpression(),
                    new CodeObjectCreateExpression("System.ComponentModel.PropertyChangedEventArgs",
                        new CodeExpression[] { new CodePrimitiveExpression(propertyName) })
                };

                // Create delegate invoke expression and add it to the property's set statements; call PropertyChanged.
                CodeDelegateInvokeExpression invokePropertyChanged = new CodeDelegateInvokeExpression(
                    propertyChangedTargetObject, propertyChangedParameters);
                dynamicProperty.SetStatements.Add(invokePropertyChanged);

                // Add property to class.
                dynamicClass.Members.Add(dynamicProperty);
            }

            // Add class to namespace.
            dynamicNamespace.Types.Add(dynamicClass);

            // Add namespace to compile unit.
            compileUnit.Namespaces.Add(dynamicNamespace);

            // Generate CSharp code from compile unit.
            StringWriter stringWriter = new StringWriter();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            provider.GenerateCodeFromCompileUnit(compileUnit, stringWriter, new CodeGeneratorOptions());
            stringWriter.Close();
            return stringWriter.ToString();
        }

        public static object InstantiateClassFromCodeString(string codeString, string fullyQualifiedTypeName)
        {
            CSharpCodeProvider compiler = new CSharpCodeProvider();
            CompilerParameters compilerParams = new CompilerParameters(new string[] { "System.dll" });
            CompilerResults results = compiler.CompileAssemblyFromSource(compilerParams, new string[] { codeString });
            return results.CompiledAssembly.CreateInstance(fullyQualifiedTypeName);
        }

        ////Example Usage:
        //Hashtable rawCustomer = new Hashtable();
        //rawCustomer["Id"] = 123456789;
        //rawCustomer["FirstName"] = "Billy";
        //rawCustomer["LastName"] = "Bob";
        //rawCustomer["Rating"] = 84.5D;

        //string customerCode = CodeGenerationHelper.CreateClassFromHashtable("CodeGenerationTest", "Customer",
        //    rawCustomer, true);
        //        object customer = CodeGenerationHelper.InstantiateClassFromCodeString(customerCode, "CodeGenerationTest.Customer");

    }
}
