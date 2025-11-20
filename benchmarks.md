## sin ifs con "??""

| Method                                          | Mean       | Error     | StdDev    |
|------------------------------------------------ |-----------:|----------:|----------:|
| Default_Singleton_Simple                        |   3.475 ns | 0.0136 ns | 0.0081 ns |
| Default_Singleton_WithDependency                |   3.485 ns | 0.0426 ns | 0.0254 ns |
| Child_Singleton_Simple                          |   7.288 ns | 0.0197 ns | 0.0130 ns |
| Child_Singleton_WithDependency_AllParent        |   7.302 ns | 0.0132 ns | 0.0087 ns |
| Child_Singleton_WithDependency_AllChild         |   7.286 ns | 0.0357 ns | 0.0236 ns |
| Child_Singleton_WithDependency_ServiceParent    |   7.639 ns | 0.0265 ns | 0.0175 ns |
| Child_Singleton_WithDependency_DependencyParent |   7.234 ns | 0.0215 ns | 0.0128 ns |
| Default_Scoped_Simple                           |   4.790 ns | 0.0096 ns | 0.0057 ns |
| Default_Scoped_WithDependency                   |   5.042 ns | 0.0113 ns | 0.0059 ns |
| Child_Scoped_Simple                             |         NA |        NA |        NA |
| Child_Scoped_WithDependency_AllParent           |         NA |        NA |        NA |
| Child_Scoped_WithDependency_AllChild            |         NA |        NA |        NA |
| Child_Scoped_WithDependency_ServiceParent       |         NA |        NA |        NA |
| Child_Scoped_WithDependency_DependencyParent    |         NA |        NA |        NA |
| Default_Transient_Simple                        |   5.762 ns | 0.0184 ns | 0.0110 ns |
| Default_Transient_WithDependency                |   5.764 ns | 0.0300 ns | 0.0178 ns |
| Child_Transient_Simple                          |  47.209 ns | 0.2628 ns | 0.1738 ns |
| Child_Transient_WithDependency_AllParent        |  77.026 ns | 0.2719 ns | 0.1799 ns |
| Child_Transient_WithDependency_AllChild         | 146.923 ns | 0.6827 ns | 0.4516 ns |
| Child_Transient_WithDependency_ServiceParent    | 108.517 ns | 0.7057 ns | 0.4199 ns |
| Child_Transient_WithDependency_DependencyParent |  48.307 ns | 0.0675 ns | 0.0447 ns |

## if known types

| Method                                          | Mean       | Error     | StdDev    |
|------------------------------------------------ |-----------:|----------:|----------:|
| Default_Singleton_Simple                        |   3.426 ns | 0.0229 ns | 0.0120 ns |
| Default_Singleton_WithDependency                |   3.464 ns | 0.0172 ns | 0.0103 ns |
| Child_Singleton_Simple                          |   6.528 ns | 0.0289 ns | 0.0191 ns |
| Child_Singleton_WithDependency_AllParent        |   6.726 ns | 0.1075 ns | 0.0711 ns |
| Child_Singleton_WithDependency_AllChild         |   6.619 ns | 0.0271 ns | 0.0161 ns |
| Child_Singleton_WithDependency_ServiceParent    |   6.739 ns | 0.0538 ns | 0.0356 ns |
| Child_Singleton_WithDependency_DependencyParent |   6.585 ns | 0.0228 ns | 0.0151 ns |
| Default_Scoped_Simple                           |   4.788 ns | 0.0106 ns | 0.0063 ns |
| Default_Scoped_WithDependency                   |   5.040 ns | 0.0183 ns | 0.0121 ns |
| Child_Scoped_Simple                             |         NA |        NA |        NA |
| Child_Scoped_WithDependency_AllParent           |         NA |        NA |        NA |
| Child_Scoped_WithDependency_AllChild            |         NA |        NA |        NA |
| Child_Scoped_WithDependency_ServiceParent       |         NA |        NA |        NA |
| Child_Scoped_WithDependency_DependencyParent    |         NA |        NA |        NA |
| Default_Transient_Simple                        |   5.818 ns | 0.0250 ns | 0.0149 ns |
| Default_Transient_WithDependency                |   5.770 ns | 0.0252 ns | 0.0150 ns |
| Child_Transient_Simple                          |  48.798 ns | 0.1089 ns | 0.0648 ns |
| Child_Transient_WithDependency_AllParent        |  79.157 ns | 0.5422 ns | 0.2836 ns |
| Child_Transient_WithDependency_AllChild         | 146.778 ns | 1.0092 ns | 0.6675 ns |
| Child_Transient_WithDependency_ServiceParent    | 111.231 ns | 0.5154 ns | 0.3067 ns |
| Child_Transient_WithDependency_DependencyParent |  50.537 ns | 0.1111 ns | 0.0735 ns |