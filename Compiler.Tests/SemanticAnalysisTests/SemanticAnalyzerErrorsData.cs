using System.Collections;

namespace Compiler.Tests.SemanticAnalysisTests;

public class SemanticAnalyzerErrorsData : IEnumerable<object[]>
{
     public IEnumerator<object[]> GetEnumerator()
     {
         yield return new object[]
         {
             """
             func void Main() {
                 var void a = 1;

                 return;
             }
             """,
             "Variable a: type - I; does not match void;"
         };

         yield return new object[]
         {
             """
             func void Main() {
                 var array<void> a = 1;

                 return;
             }
             """,
             "Variable a: type - I; does not match [void;"
         };

         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> a = 1;

                 return;
             }
             """,
             "Variable a: type - I; does not match [I;"
         };

         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> a;

                 return;
             }
             """,
             "You must provide an array size"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var int a = new int(5)[];

                 return;
             }
             """,
             "Variable a: type - [I; does not match I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var int a;
                 return;
             }

             """,
             "Variable a is not initialized"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var int a;
                 print a;
                 a = 9;
                 return;
             }

             """,
             "Variable a is not initialized"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var int a;
                 return;
             }

             """,
             "Variable a is not initialized"
         };

         yield return new object[]
         {
             """
             func void Main() {
                 var int a;
                 return;
             }

             """,
             "Variable a is not initialized"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var int a = arr;
                 return;
             }

             """,
             "Variable a: type - [I; does not match I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 return 1;
             }

             """,
             "Function Main expected return type is void; but got I;"
         };
         
         yield return new object[]
         {
             """
             func int foo() {
                 return;
             }
             
             func void Main() {
                 return;
             }

             """,
             "Function foo expected return type is I; but got void;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var int a;
                 a = arr;
                 return;
             }

             """,
             "Assigment: identifier have type: I; but got type: [I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var array<int> arr2 = new int(5)[];
                 arr[0] = arr2;
                 return;
             }

             """,
             "Assigment: array have type: I; but got type: [I;"
         };

         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var array<int> arr2 = new int(arr)[];
                 return;
             }

             """,
             "Array creation: size type is [I; but must be int"
         };
         
          yield return new object[]
          {
              """
              func void foo() {
                  return;
              }
              
              func void Main() {
                  var array<int> arr = new int(foo)[];
                  return;
              }

              """,
              "Functions as types are not supported now"
          };

         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var array<int> arr2 = new int(arr)[];
                 return;
             }

             """,
             "Array creation: size type is [I; but must be int"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var array<int> arr2 = new int(3)[];
                 arr2[arr] = 5;
                 return;
             }

             """,
             "Array index: index type is [I; but must be int"
         };
         
         yield return new object[]
         {
             """
             func void foo() {
                 return;
             }
             
             func void Main() {
                 var array<int> arr = new int(5)[];
                 arr[foo] = 5;
                 return;
             }

             """,
             "Functions as types are not supported now"
         };

         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var int n = 5;
                 var int res = n + arr;
                 return;
             }

             """,
             "Now arithmetic operations only for int. Got: I; and [I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var int n = 5;
                 var int m = 9;
                 var int res = (n + m) * arr;
                 return;
             }

             """,
             "Now arithmetic operations only for int. Got: I; and [I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var int n = 5;
                 var int m = 9;
                 arr = n + m;
                 return;
             }

             """,
             $"Assigment: identifier have type: [I; but got type: I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var array<int> arr = new int(5)[];
                 var array<int> arr2 = new int(5)[];
                 arr2 = -arr;
                 return;
             }

             """,
             "Unsupported unary operation for operator Minus and type [I;"
         };
         
         yield return new object[]
         {
             """
             func void foo() {
                  return;
             }
             
             func void Main() {
                 foo = 8;
                 return;
             }

             """,
             "Expected that foo is a variable"
         };
        
         yield return new object[]
         {
             """
             func void Main() {
                 var int a = 5;
                 a();
                 return;
             }

             """,
             "a is not function. You can call only function"
         };

         yield return new object[]
         {
             """
             func void MyPrint(int a) {
                print a;
                return;
             }
             func void Main() {
                 MyPrint();
                 return;
             }

             """,
             $"Function MyPrint must have the same number of parameters - 1"
         };
         
         yield return new object[]
         {
             """
             func void MyPrint(int a) {
                print a;
                return;
             }
             func void Main() {
                 var array<int> arr = new int(5)[];
                 MyPrint(arr);
                 return;
             }

             """,
             $"Invalid argument 'a' type. Expected 'I;' but found '[I;'"
         };

         yield return new object[]
         {
             """
             func int Sum(int a, int b) {
                return a + b;
             }
             func void Main() {
                 var array<int> arr = new int(5)[];
                 arr = Sum(1, 2);
                 return;
             }

             """,
             "Assigment: identifier have type: [I; but got type: I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 1;
                 return;
             }

             """,
             "Result of expression must be used"
         };

         yield return new object[]
         {
             """
             func int foo() {
                return 1;
             }
             
             func void Main() {
                 foo();
                 return;
             }

             """,
             "Result of expression must be used"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var bool flag = 1;
                 return;
             }

             """,
             $"Variable flag: type - I; does not match B;"
         };

         yield return new object[]
         {
             """
             func void Main() {
                 var bool flag = false;
                 flag = flag + 1;
                 return;
             }

             """,
             $"Now arithmetic operations only for int. Got: B; and I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var bool flag = false;
                 flag = flag && 1;
                 return;
             }

             """,
             $"Logical operations only for bool. Got: B; and I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var bool flag = false;
                 flag = flag == 1;
                 return;
             }

             """,
             "Different types in comparison: B; and I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 var bool flag = !1;
                 return;
             }

             """,
             $"Unsupported unary operation for operator Negation and type I;"
         };
         
         yield return new object[]
         {
             """
             func void Main() {
                 if (1) {
                    print 1;
                 }
                 return;
             }

             """,
             "If statement: condition type is I;. Expected: B;"
         };
         
          yield return new object[]
          {
              """
              func void foo() {
                return;
              }
              func void Main() {
                  while (foo()) {
                     print 1;
                  }
                  return;
              }

              """,
              "While statement: condition type is void;. Expected: B;"
          };
          
          yield return new object[]
          {
              """
              func void Main() {
                  for (var int i; i < 5; i = i + 1) {
                     print i;
                  }
                  return;
              }

              """,
              "For statement: you must init variable"
          };
          
          yield return new object[]
          {
              """
              func void Main() {
                  for (var int i = 0; i + 5; i = i + 1) {
                     print i;
                  }
                  return;
              }

              """,
              $"For statement: condition type is I;. Expected: B;"
          };
          
          yield return new object[]
          {
              """
              func void Main() {
                  break;
                  return;
              }

              """,
              "Break statement is not in loop"
          };
          
          yield return new object[]
          {
              """
              func void Main() {
                  if (1 == 1) { 
                      print 1;
                      break;
                  }
                  return;
              }
              """,
              "Break statement is not in loop"
          };
          
          yield return new object[]
          {
              """
              func void Main() {
                  var int64 b = 10L;
                  var int a = b;
                  return;
              }
              """,
              "Variable a: type - I8; does not match I;"
          };
          
          yield return new object[]
          {
              """
              func void f(int a) {
                print a;
                return;
              }
              func void Main() {
                  var int64 a = 10L;
                  f(a);
                  return;
              }
              """,
              "Invalid argument 'a' type. Expected 'I;' but found 'I8;'"
          };
          
          yield return new object[]
          {
              """
              func void f(int a) {
                print a;
                return;
              }
              func void Main() {
                  var int64 a = 10L;
                  var int64 b = 100L;
                  var int c = a / b;
                  return;
              }
              """,
              "Variable c: type - I8; does not match I;"
          };
    }

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
}