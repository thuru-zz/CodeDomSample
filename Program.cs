using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace CodeDOM
{
    class Program
    {
        private static void Main(string[] args)
        {
            // SUPPORTED LANGUAGES 
            foreach (System.CodeDom.Compiler.CompilerInfo ci in System.CodeDom.Compiler.CodeDomProvider.GetAllCompilerInfo())
            {
                foreach (string language in ci.GetLanguages())
                {
                    System.Console.WriteLine("{0} ", language);
                }
            }

            CodeNamespace codenamespace = BuildProgram();
            
            var options = new CodeGeneratorOptions()
            {
                IndentString = "    ",
                BracingStyle = "C",
                BlankLinesBetweenMembers = true
            };

            using (var codeWriter = new StreamWriter(@"Student.cs") { AutoFlush = true })
            {
                CodeDomProvider.CreateProvider("c#").GenerateCodeFromNamespace(codenamespace, codeWriter, options);
            }

            
            Console.ReadKey();
        }


        private static CodeNamespace BuildProgram()
        {
            // create the namespace
            var ns = new CodeNamespace("StudentManagement");

            // add comment to the namespace
            ns.Comments.Add(new CodeCommentStatement()
            {
                Comment = new CodeComment()
                {
                    DocComment = true,
                    Text = "This is auto generated code. Modifications to the code might result breaking changes.",
                }
            });

            // add imports to the namespace
            ns.Imports.AddRange(new CodeNamespaceImport[] 
            {
                new CodeNamespaceImport("System"),
                new CodeNamespaceImport("System.IO")
            });



            // declare a type
            var studentClass = new CodeTypeDeclaration("Student");
            studentClass.IsClass = true;

            // attribute declaration and it'sa better practice to make the auto generated classes as partial
            studentClass.TypeAttributes = TypeAttributes.Public;
            studentClass.IsPartial = true;
           

            // add a simple property
            var myproperty = new CodeMemberProperty
            {
                // by default all members would be virtual, add MemberAttributes.Final to make the non virtual
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "FullName",
                Type = new CodeTypeReference(typeof(string)),
                HasSet = true,
            };

            // add the property to the class
            studentClass.Members.Add(myproperty);


            // creating a private property
            var field = new CodeMemberField(typeof(int), "_studentRegId");
            field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

            // add the property to the class
            studentClass.Members.Add(field);


            // creating the constructor
            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            // creating a parameter
            var regId = new CodeParameterDeclarationExpression(typeof(int), "regId");

            // add the parameter to the constructor
            constructor.Parameters.Add(regId);
            
            // create assignment logic
            var idAssignment = new CodeAssignStatement
            (
                    new CodeVariableReferenceExpression("this._studentRegId"),
                    new CodeVariableReferenceExpression("regId")
            );

            // adding assignment to the constructor
            constructor.Statements.Add(idAssignment);



            // add constructor to the class
            studentClass.Members.Add(constructor);

            // creating a public method
            var printMethod = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "PrintStudentDetail"
            };

            // creating an attribute
            var attribute = new CodeAttributeDeclaration("OperationContract");

            // add attribute to the method
            printMethod.CustomAttributes = new CodeAttributeDeclarationCollection(new CodeAttributeDeclaration[] { attribute });

            // add print method logic
            printMethod.Statements.Add
            (
                new CodeMethodInvokeExpression
                (
                    new CodeSnippetExpression("Console"), "WriteLine", new CodePrimitiveExpression("String.Format(\"{0}'s student ID - {1}\", FullName, _studentRegId)")
                )
            );

            // adding method to the class
            studentClass.Members.Add(printMethod);


            // add class to the namespace
            ns.Types.Add(studentClass);

            return ns;
        }

    }
   
}
