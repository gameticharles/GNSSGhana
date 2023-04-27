using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;


namespace ghGPS.Classes
{
    public class ClassBuilder
    {
        /// <summary>
        /// The Assembly Name class in the System.Reflection namespace initializes a new instance of the System.Reflection.AssemblyName 
        /// </summary>
        AssemblyName asemblyName;
        public ClassBuilder(string ClassName)
        {
            this.asemblyName = new AssemblyName(ClassName);
        }

        public object CreateObject(string[] PropertyNames, Type[] Types)
        {
            if (PropertyNames.Length != Types.Length)
            {
                Console.WriteLine("The number of property names should match their corresopnding types number");
            }

            TypeBuilder DynamicClass = this.CreateClass();
            this.CreateConstructor(DynamicClass);
            for (int ind = 0; ind < PropertyNames.Count(); ind++)
                CreateProperty(DynamicClass, PropertyNames[ind], Types[ind]);
            Type type = DynamicClass.CreateType();

            return Activator.CreateInstance(type);
        }

        private TypeBuilder CreateClass()
        {
            //This class helps to create a dynamic assembly at run time. This is a sealed class and has no constructor. The object of this class is created using the DefineDynamicAssembly method of the AppDomain 
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(this.asemblyName, AssemblyBuilderAccess.Run);

            //This class dynamically creates a module in the project. It inherits from the Module class in System.Reflection.Emit and impliments a _ModuleBuilder interface. This class also has no constructor, 
            //hence an object of the class can be created using the DefineDynamicModule function of the assembly builder class
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            //TypeBuilder creates a new instance of the class specified by the name and inherits from System.Reflection.TypeInfo. 
            //This is also a sealed class and cannot be directly created by an object of this class. It comes from the System.Reflection.Emit namespace,
            //the function DefineType of the ModuleBuilder class creates an object of this class.
            TypeBuilder typeBuilder = moduleBuilder.DefineType(this.asemblyName.FullName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return typeBuilder;
        }
        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }

        /// <summary>
        /// Creating a property is a little bit of a long process. To create a property of a class,
        /// we need to define the field of that property and the get and set methods, 
        /// the following code creates a class's property with their get and set method.
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            //This class cannot be inherited and used to create a field or variable of a class. It inherits from the System.Reflection.FieldInfo class. 
            //The DefineField function of the typebilder class creates an object of this class.
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            
            //This class defines or creates a property of a class at runtime, it is in the System.Reflection.Emit namespace and cannot be inherited. This class inherites from the System.Reflection.PropertyInfo class. 
            //The DefineProperty function of the typebuilder class creates an object of the PropertyBuilder class.
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

    }
}
