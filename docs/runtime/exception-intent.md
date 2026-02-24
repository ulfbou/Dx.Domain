# Exception Intent Classifier (DXA022 companion)

**Buckets**: ArgumentValidation (ArgumentException family), InvariantViolation (InvariantViolationException), ControlFlow (OperationCanceled/TaskCanceled), DomainControl (configurable), Unknown.  
**Policy**: DXA022 flags only **DomainControl** exceptions escaping public `Result` methods in consumer code. Guards/invariants/rethrows are allowed.
