using System.Collections;

namespace Compiler.Tests.TaskTests;

public class PipelineTestsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            """
            func int64 Factorial(int64 i) {
                if (i <= 1L) {
                    return 1L;
                } 
                return Factorial(i - 1L) * i;
            }

            func void Main() {
                var int64 res = Factorial(20L);
                print res;
                
                return;
            }
            """
        };

        yield return new object[]
        {
            """
            =/
            Merge Sort Implementation
            /=
            func void Merge(array<int> arr, int left, int mid, int right) {
                var int it1 = 0;
                var int it2 = 0;
                var array<int> result = new int(right - left)[]; // create array with size right - left
            
                while (left + it1 < mid && mid + it2 < right) {
                      if (arr[left + it1] <= arr[mid + it2]) {
                          result[it1 + it2] = arr[left + it1];
                          it1 = it1 + 1;
                      } else {
                          result[it1 + it2] = arr[mid + it2];
                          it2 = it2 + 1;
                      }
                }
            
                while (left + it1 < mid) {
                    result[it1 + it2] = arr[left + it1];
                    it1 = it1 + 1;
                }
                                
                while (mid + it2 < right) {
                    result[it1 + it2] = arr[mid + it2];
                    it2 = it2 + 1;
                }
            
                for (var int i = 0; i < it1 + it2; i = i + 1) {
                    arr[left + i] = result[i];
                }
                return;
            }
            
            func void MergeSort(array<int> arr, int left, int right) {
                if (left + 1 >= right) {
                  return;
                }
            
                var int mid = (left + right) / 2;
                MergeSort(arr, left, mid);
                MergeSort(arr, mid, right);
                Merge(arr, left, mid, right);
                return;
            }
            
            func void Main() {
                var int n = 10000;
                var array<int> arr = new int(n)[]; // create array with size n
                for (var int i = 0; i < n; i = i + 1) {
                  arr[i] = n - i;
                }
                MergeSort(arr, 0, n);
                for (var int i = 0; i < n - 1; i = i + 1) {
                    if (arr[i] > arr[i + 1]) {
                      print 0;
                      return;
                    }
                }
                print 1;
                return;
            }
            
            """
        };
        
        yield return new object[]
        {
            """
            func void S(array<bool> prime, int p, int n) {
                if (prime[p] == true) {
                    for (var int i = p * p; i <= n; i = i + p) {
                        prime[i] = false;
                    }
                }
                return;
            }
            
            func void Main() {
                var int n = 1000000;
                var array<bool> prime = new bool(n + 1)[];
                for (var int i = 2; i < n + 1; i = i + 1) {
                    prime[i] = true;
                }
            
                for (var int p = 2; p * p <= n; p = p + 1) {
                    S(prime, p, n);
                }
            
                for (var int i = 2; i <= n; i = i + 1) {
                    if (prime[i] == true) {
                        print i;
                        return;
                    }
                }
                return;
            }
            """
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int n = 1000000;
                var int sum = 0;
                for (var int i = 2; i < n + 1; i = i + 1) {
                    var int x = 2;
                    var int a = 10 * 10 - 9;
                    a = x + 2;
                    a = x * 2;
                    a = 10;
                    var int b = 3;
                    var int c = 13;
                    var int d = 23;
                    var int e = 37;
                    var int f = 30;
                    var int g = 3;
                    var int h = 36;
                    var int j = 93;
                    j = 1;
                    j = 2;
                    j = 3;
                    j = 10 % 4;
                    j = 10 / 4;
                    j = 10 * 4;
                    sum = sum + i;
                    print i;
                    continue;
                    var int k = 10;
                    var int l = k * 2;
                    k = k + 1;
                    l = k + 7;
                    l = l / 6;
                    k = 10;
                    print k;
                    print l;
                }
                return;
            }
            """
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}