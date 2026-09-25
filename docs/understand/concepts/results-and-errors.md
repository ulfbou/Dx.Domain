# Results and Errors

`Result<T>` represents success or a `DomainError`. Create values through `Dx.Result.Success` and `Dx.Result.Failure`, then return, transform, or handle the result explicitly. Result shape is compiler-enforced; observation patterns are analyzer-enforced; correctness of handlers is not guaranteed.

See [Limitations](../../use/limitations.md).
