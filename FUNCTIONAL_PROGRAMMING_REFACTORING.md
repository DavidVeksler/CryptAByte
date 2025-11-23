# Functional Programming Refactoring Guide

## Overview

This document describes the functional programming refactoring of the CryptAByte codebase. The refactoring transforms imperative, mutation-heavy code into pure, composable functions with explicit data flow and isolated I/O operations.

## Core Principles

### 1. **Pure Functions**
- Functions always return the same output for the same input
- No hidden dependencies (like `DateTime.Now`, static RNG calls)
- No side effects (no mutations, no I/O)
- Deterministic and easily testable

### 2. **Immutable Data Structures**
- All domain models are immutable after construction
- Changes create new instances rather than mutating existing ones
- Thread-safe by default
- Enables safe sharing across contexts

### 3. **Explicit Data Flow**
- All dependencies are injected (time provider, random generator, crypto functions)
- No output parameters - use return values or Result types
- Function signatures clearly show all inputs and outputs

### 4. **I/O Isolation**
- Pure business logic separated from I/O operations
- Database, file system, network, and RNG operations pushed to boundaries
- Easy to test business logic without I/O infrastructure

### 5. **Explicit Error Handling**
- `Result<TValue, TError>` type for operations that can fail
- `Option<T>` type for optional values (eliminates null reference exceptions)
- No silent exception handling - all errors are explicit in function signatures

## Refactoring Completed

### Core Functional Types

#### **1. Result&lt;TValue, TError&gt;** (`/CryptAByte.Domain/Functional/Result.cs`)

A discriminated union representing success or failure:

```csharp
// Creating results
var success = Result.Success<int, string>(42);
var failure = Result.Failure<int, string>("Something went wrong");

// Transforming results
var doubled = success.Map(x => x * 2);  // Success(84)

// Chaining operations
var result = GetUser(id)
    .Bind(user => GetUserPreferences(user.Id))
    .Map(prefs => prefs.Theme);

// Pattern matching
result.Match(
    onSuccess: theme => Console.WriteLine($"Theme: {theme}"),
    onFailure: error => Console.WriteLine($"Error: {error}")
);
```

**Key Methods:**
- `Map<TResult>()` - Transform success value
- `Bind<TResult>()` - Chain operations that return Result
- `Match<TResult>()` - Pattern match on success/failure
- `Sequence()` - Convert `IEnumerable<Result<T>>` to `Result<IEnumerable<T>>`

#### **2. Option&lt;T&gt;** (`/CryptAByte.Domain/Functional/Option.cs`)

Represents an optional value (explicit null handling):

```csharp
// Creating options
var some = Option.Some("value");
var none = Option.None<string>();

// From nullable
var option = Option.FromNullable(nullableString);

// Transforming
var upper = some.Map(s => s.ToUpper());  // Some("VALUE")

// Pattern matching
option.Match(
    onSome: value => Console.WriteLine(value),
    onNone: () => Console.WriteLine("No value")
);

// Get value or default
var value = option.GetValueOrDefault("default");
```

**Key Methods:**
- `Map<TResult>()` - Transform the value if present
- `Bind<TResult>()` - Chain operations that return Option
- `Where()` - Filter based on predicate
- `Match<TResult>()` - Pattern match on Some/None

#### **3. ITimeProvider** (`/CryptAByte.Domain/Functional/ITimeProvider.cs`)

Abstracts current time to eliminate temporal coupling:

```csharp
public interface ITimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}

// Production use
var timeProvider = new SystemTimeProvider();

// Testing use
var timeProvider = new FixedTimeProvider(new DateTime(2025, 1, 1));
```

#### **4. IRandomGenerator** (`/CryptAByte.Domain/Functional/IRandomGenerator.cs`)

Abstracts random number generation:

```csharp
public interface IRandomGenerator
{
    byte[] GenerateBytes(int length);
    string GenerateBase64String(int sizeInBytes);
}

// Production use
var rng = new CryptoRandomGenerator();

// Testing use
var rng = new DeterministicRandomGenerator(0x42);
```

### Refactored Cryptographic Providers

#### **AsymmetricCryptoProvider** (`/CryptAByte.CryptoLibrary/CryptoProviders/AsymmetricCryptoProvider.cs`)

**Before:**
```csharp
// Output parameters, hidden dependencies
string EncryptMessageWithKey(string message, string publicKey,
    out string encryptedPassword, out string hashOfMessage)
{
    string encryptionKey = SymmetricCryptoProvider.GenerateKeyPhrase(); // Static call!
    // ...
}
```

**After:**
```csharp
// Explicit dependencies, immutable result
public AsymmetricCryptoProvider(
    SymmetricCryptoProvider symmetricProvider,
    IRandomGenerator randomGenerator)

public AsymmetricEncryptionResult EncryptMessageWithKey(
    string message, string publicKey)
{
    var encryptionKey = _randomGenerator.GenerateBase64String(128);
    // Returns immutable AsymmetricEncryptionResult
}

// Explicit error handling
public Result<DecryptedData, string> DecryptMessageWithKey(
    string privateKey, string messageData,
    string encryptedDecryptionKey, string hashOfMessage)
{
    // Returns Result instead of throwing exceptions
}
```

**Improvements:**
- ✅ Eliminated output parameters
- ✅ Injected dependencies (SymmetricCryptoProvider, IRandomGenerator)
- ✅ Returns immutable value objects
- ✅ Explicit error handling with Result type
- ✅ Legacy methods preserved with `[Obsolete]` attribute

#### **SymmetricCryptoProvider** (`/CryptAByte.CryptoLibrary/CryptoProviders/SymmetricCryptoProvider.cs`)

**Improvements:**
- ✅ Marked `GenerateKeyPhrase()` as obsolete (use IRandomGenerator instead)
- ✅ Added pure `CryptoFunctions` static class for hash operations
- ✅ Input validation on all methods
- ✅ Documented purity guarantees

### Immutable Value Objects

#### **CryptoTypes** (`/CryptAByte.Domain/Functional/CryptoTypes.cs`)

Immutable data structures for cryptographic operations:

- `EncryptedData` - Cipher text + IV
- `AsymmetricEncryptionResult` - Encrypted message, key, hash, IV
- `DecryptedData` - Plain text, decryption key, hash
- `KeyPair` - RSA public/private key pair
- `ProtectedKeyPair` - Passphrase-encrypted key pair
- `FileAttachment` - File name, data, Base64 ZIP

### Refactored Utilities

#### **FileUtilities** (`/CryptAByte.Domain/Utilities/FileUtilities.cs`)

**Before:**
```csharp
byte[] DecodeAndDecompressFile(string base64ZippedData, out string fileName)
{
    // Output parameter, exceptions for errors
}
```

**After:**
```csharp
Result<DecompressedFile, string> DecodeAndDecompressFile(string base64ZippedData)
{
    // Returns immutable DecompressedFile or error
}
```

**Improvements:**
- ✅ Eliminated output parameters
- ✅ Returns `Result<T, string>` for explicit error handling
- ✅ Immutable `DecompressedFile` value object
- ✅ All methods are pure transformations
- ✅ Legacy methods preserved with `[Obsolete]` attribute

### Immutable Domain Models

#### **ImmutableDomainModels** (`/CryptAByte.Domain/Models/ImmutableDomainModels.cs`)

Immutable versions of EF entities for business logic:

- `ImmutableCryptoKey` - Immutable crypto key with pure methods
- `ImmutableMessage` - Immutable message
- `ImmutableNotification` - Immutable notification

**Key Features:**
```csharp
// Pure transformation methods
public ImmutableCryptoKey WithMessages(IEnumerable<ImmutableMessage> newMessages);
public ImmutableCryptoKey WithoutPrivateKey();

// Pure validation
public bool IsReleased(DateTime currentTime);
public Option<string> GetPrivateKeyIfReleased(DateTime currentTime);

// Conversions at I/O boundary
public static ImmutableCryptoKey FromEntity(CryptoKey entity);
public CryptoKey ToEntity();
```

#### **ImmutableSelfDestructingMessage** (`/CryptAByte.Domain/Models/ImmutableSelfDestructingMessage.cs`)

Immutable self-destructing messages:

- `ImmutableSelfDestructingMessage`
- `ImmutableSelfDestructingMessageAttachment`

### Pure Business Logic

#### **MessageOperations** (`/CryptAByte.Domain/BusinessLogic/MessageOperations.cs`)

Pure functions for message operations:

```csharp
// Decrypt and decompress without mutations
Result<ImmutableMessage, string> DecryptAndDecompress(
    ImmutableMessage encryptedMessage,
    string privateKey,
    Func<...> decryptFunction);

// Validate key for reading
Result<ImmutableCryptoKey, string> ValidateKeyForReading(
    ImmutableCryptoKey cryptoKey,
    DateTime currentTime);

// Decrypt private key with passphrase
Result<string, string> DecryptPrivateKey(
    ImmutableCryptoKey cryptoKey,
    string passphrase,
    Func<...> decryptFunction,
    Func<...> hashFunction);
```

#### **CryptoKeyOperations** (`/CryptAByte.Domain/BusinessLogic/MessageOperations.cs`)

Pure functions for crypto key operations:

```csharp
// Create keys with explicit dependencies
ImmutableCryptoKey CreateWithGeneratedKeys(
    string keyToken,
    DateTime requestDate,
    DateTime releaseDate,
    Func<KeyPair> generateKeysFunction);

// Pure transformation
ImmutableCryptoKey MakePublicKeyOnly(ImmutableCryptoKey cryptoKey);
```

## Migration Guide for Remaining Code

### Refactoring Repositories

The repositories need the most significant refactoring. Follow this pattern:

#### **1. Separate Commands from Queries**

```csharp
// BEFORE: Mixed concerns
public class RequestRepository
{
    public CryptoKey GetByToken(string token) { }  // Query
    public void AttachMessage(string token, ...) { }  // Command
}

// AFTER: Separate interfaces
public interface IRequestQueries
{
    Result<ImmutableCryptoKey, string> GetByToken(string token);
    Result<IReadOnlyList<ImmutableMessage>, string> GetMessages(string token);
}

public interface IRequestCommands
{
    Result<Unit, string> SaveCryptoKey(ImmutableCryptoKey key);
    Result<Unit, string> AttachMessage(string token, ImmutableMessage message);
    Result<Unit, string> DeleteMessages(IEnumerable<int> messageIds);
}
```

#### **2. Extract Pure Business Logic**

```csharp
// BEFORE: Business logic mixed with I/O
public void AttachMessageToRequest(string token, string message, string publicKey)
{
    var request = _context.Keys.Find(token);  // I/O

    var hash = SymmetricCryptoProvider.GetSecureHashForString(message);  // Logic
    var encrypted = crypto.EncryptMessageWithKey(message, publicKey,
        out var key, out var hash);  // Logic

    request.Messages.Add(new Message { ... });  // Mutation + I/O
    _context.SaveChanges();  // I/O
}

// AFTER: Separate pure logic from I/O
// Pure function (in BusinessLogic/)
public static Result<EncryptedMessageData, string> EncryptMessage(
    string plaintext,
    string publicKey,
    IRandomGenerator randomGenerator,
    AsymmetricCryptoProvider crypto)
{
    // Pure transformation, returns immutable result
}

// Repository (I/O boundary)
public Result<Unit, string> AttachMessage(
    string token,
    EncryptedMessageData encryptedData)
{
    // Just persist the data, no business logic
}
```

#### **3. Eliminate ForEach Mutations**

```csharp
// BEFORE: ForEach with side effects
request.Messages.ToList().ForEach(msg => {
    msg.MessageData = DecryptMessage(msg);  // MUTATION
    msg.EncryptionKey = key;  // MUTATION
});

// AFTER: Map to new collection
var decryptedMessages = request.Messages
    .Select(msg => ImmutableMessage.FromEntity(msg))
    .Select(msg => MessageOperations.DecryptAndDecompress(
        msg, privateKey, decryptFunction))
    .Sequence()  // Convert List<Result<T>> to Result<List<T>>
    .Map(messages => messages.ToList().AsReadOnly());
```

#### **4. Inject Time Dependencies**

```csharp
// BEFORE: Hidden dependency
public bool IsReleased() => ReleaseDate < DateTime.Now;  // Not testable!

// AFTER: Explicit dependency
public RequestRepository(
    CryptAByteContext context,
    ITimeProvider timeProvider,
    IEmailService emailService)
{
    _timeProvider = timeProvider;
}

public Result<ImmutableCryptoKey, string> GetReleasedKey(string token)
{
    return GetByToken(token)
        .Bind(key => MessageOperations.ValidateKeyForReading(
            key, _timeProvider.UtcNow));
}
```

### Refactoring Controllers

Controllers should be thin adapters between HTTP and business logic:

```csharp
// BEFORE: Business logic in controller
public ActionResult GetMessages(string token, string passphrase)
{
    try
    {
        var request = _repo.GetByToken(token);
        if (request.ReleaseDate > DateTime.Now)
            return new HttpStatusCodeResult(403);

        var privateKey = new SymmetricCryptoProvider()
            .DecryptWithKey(request.PrivateKey, passphrase);

        request.Messages.ToList().ForEach(msg => {
            msg.MessageData = DecryptMessage(msg, privateKey);
        });

        return View(request);
    }
    catch (Exception ex)
    {
        return new HttpStatusCodeResult(500);
    }
}

// AFTER: Thin adapter with pure business logic
public ActionResult GetMessages(string token, string passphrase)
{
    var result = _queries.GetByToken(token)
        .Bind(key => MessageOperations.ValidateKeyForReading(
            key, _timeProvider.UtcNow))
        .Bind(key => MessageOperations.DecryptPrivateKey(
            key, passphrase, _crypto.DecryptWithKey,
            SymmetricCryptoProvider.GetSecureHashForString))
        .Bind(privateKey => _queries.GetMessages(token)
            .Bind(messages => MessageOperations.DecryptAndDecompressAll(
                messages, privateKey, _crypto.DecryptMessageWithKey)));

    return result.Match(
        onSuccess: messages => View(messages),
        onFailure: error => BadRequest(error)
    );
}
```

### Global State Refactoring

#### **Application Cache**

```csharp
// BEFORE: Global mutable state
internal static Dictionary<string, TemporaryDownloadKey> FilePasswords
{
    get
    {
        var cache = HttpRuntime.Cache;
        if (cache.Get(keyName) == null)
            cache[keyName] = new Dictionary<...>();  // SIDE EFFECT
        return cache[keyName] as Dictionary<...>;
    }
}

// AFTER: Explicit state service
public interface IDownloadTokenService
{
    Result<DownloadToken, string> CreateToken(
        string messageId,
        byte[] fileData,
        DateTime expiresAt);

    Result<DownloadToken, string> GetToken(string tokenId);
}

public class MemoryCachedDownloadTokenService : IDownloadTokenService
{
    private readonly ITimeProvider _timeProvider;

    public Result<DownloadToken, string> CreateToken(...)
    {
        // Immutable DownloadToken objects
        // Explicit expiration using ITimeProvider
    }
}
```

## Testing Strategy

The functional refactoring makes testing much easier:

### Testing Pure Functions

```csharp
[Test]
public void DecryptMessage_WithValidData_ReturnsDecryptedMessage()
{
    // Arrange
    var message = new ImmutableMessage(...);
    var privateKey = "test-key";
    var mockDecrypt = (string pk, string data, string key, string hash) =>
        Result.Success<DecryptedData, string>(new DecryptedData(...));

    // Act
    var result = MessageOperations.DecryptAndDecompress(
        message, privateKey, mockDecrypt);

    // Assert
    Assert.That(result.IsSuccess, Is.True);
    result.Match(
        onSuccess: msg => Assert.That(msg.MessageData, Is.EqualTo("expected")),
        onFailure: _ => Assert.Fail()
    );
}
```

### Testing with Fixed Time

```csharp
[Test]
public void ValidateKeyForReading_BeforeReleaseDate_ReturnsFailure()
{
    // Arrange
    var releaseDate = new DateTime(2025, 12, 31);
    var currentTime = new DateTime(2025, 1, 1);  // Before release
    var key = new ImmutableCryptoKey(..., releaseDate, ...);

    // Act
    var result = MessageOperations.ValidateKeyForReading(key, currentTime);

    // Assert
    Assert.That(result.IsFailure, Is.True);
}
```

### Testing with Deterministic Randomness

```csharp
[Test]
public void EncryptMessage_WithDeterministicRng_ProducesPredictableOutput()
{
    // Arrange
    var rng = new DeterministicRandomGenerator(0x42);
    var crypto = new AsymmetricCryptoProvider(symmetric, rng);

    // Act
    var result1 = crypto.EncryptMessageWithKey(message, publicKey);
    var result2 = crypto.EncryptMessageWithKey(message, publicKey);

    // Assert - same inputs = same outputs with deterministic RNG
    Assert.That(result1.EncryptedKey, Is.EqualTo(result2.EncryptedKey));
}
```

## Backward Compatibility

All refactored code maintains backward compatibility through:

1. **Legacy method overloads** marked with `[Obsolete]`
2. **Adapter methods** that wrap new functional code
3. **Gradual migration** - old and new code coexist

Example:
```csharp
// New functional API
public Result<DecryptedData, string> DecryptMessageWithKey(...);

// Legacy API (preserved)
[Obsolete("Use DecryptMessageWithKey returning Result<T> for explicit error handling")]
public string DecryptMessageWithKey(..., out string encryptionKey)
{
    var result = DecryptMessageWithKey(...);
    return result.Match(
        onSuccess: data => { encryptionKey = data.DecryptionKey; return data.PlainText; },
        onFailure: error => throw new CryptographicException(error)
    );
}
```

## Benefits Achieved

### **1. Testability**
- Pure functions can be tested in isolation
- No mocking required for business logic
- Deterministic tests with fixed time and random generators

### **2. Maintainability**
- Functions have clear, explicit contracts
- No hidden dependencies or side effects
- Easy to understand data flow

### **3. Reliability**
- Explicit error handling eliminates silent failures
- Immutability prevents accidental state corruption
- Type safety catches errors at compile time

### **4. Composability**
- Pure functions can be freely composed
- Reusable building blocks
- Higher-order functions enable powerful abstractions

### **5. Thread Safety**
- Immutable data structures are safe to share
- No race conditions from shared mutable state
- Easier concurrent programming

## Next Steps

### High Priority

1. **Refactor RequestRepository**
   - Split into IRequestQueries and IRequestCommands
   - Extract business logic to MessageOperations
   - Eliminate all ForEach mutations
   - Use immutable domain models

2. **Refactor SelfDestructingMessageRepository**
   - Apply same patterns as RequestRepository
   - Use ImmutableSelfDestructingMessage models
   - Explicit error handling with Result types

3. **Update Controllers**
   - Inject ITimeProvider, IRandomGenerator
   - Use business logic functions from BusinessLogic/
   - Transform to/from immutable models at boundaries
   - Return Result types, pattern match for HTTP responses

### Medium Priority

4. **Refactor Application Cache**
   - Create IDownloadTokenService interface
   - Implement with immutable DownloadToken objects
   - Inject ITimeProvider for expiration checks

5. **Update Dependency Injection**
   - Register ITimeProvider, IRandomGenerator
   - Configure AsymmetricCryptoProvider with dependencies
   - Wire up new service interfaces

### Lower Priority

6. **Refactor View Models**
   - Make view models immutable
   - Create pure transformation functions

7. **Update Tests**
   - Use new functional APIs
   - Test with fixed time and deterministic RNG
   - Add tests for Result type error paths

## Code Examples

### Creating a New Crypto Key

```csharp
// Functional approach with explicit dependencies
var keyToken = UniqueIdGenerator.GetUniqueId();
var currentTime = timeProvider.UtcNow;

var cryptoKey = CryptoKeyOperations.CreateWithGeneratedKeys(
    keyToken: keyToken,
    requestDate: currentTime,
    releaseDate: currentTime.AddDays(7),
    generateKeysFunction: () => AsymmetricCryptoProvider.GenerateKeys()
);

// Save to database
var saveResult = commands.SaveCryptoKey(cryptoKey);
```

### Decrypting Messages

```csharp
// Get crypto key
var keyResult = queries.GetByToken(token);

// Validate and decrypt
var messagesResult = keyResult
    .Bind(key => MessageOperations.ValidateKeyForReading(key, timeProvider.UtcNow))
    .Bind(key => MessageOperations.DecryptPrivateKey(
        key, passphrase, crypto.DecryptWithKey, CryptoFunctions.ComputeHash))
    .Bind(privateKey => queries.GetMessages(token))
    .Bind(messages => MessageOperations.DecryptAndDecompressAll(
        messages, privateKey, crypto.DecryptMessageWithKey));

// Handle result
messagesResult.Match(
    onSuccess: messages => DisplayMessages(messages),
    onFailure: error => ShowError(error)
);
```

## Summary

This refactoring establishes a strong functional programming foundation:

- ✅ Core functional types (Result, Option)
- ✅ Time and randomness abstraction
- ✅ Pure cryptographic providers
- ✅ Immutable domain models
- ✅ Pure business logic functions
- ✅ Explicit error handling
- ✅ I/O isolation patterns
- ✅ Backward compatibility

The remaining work involves applying these patterns throughout the controllers, repositories, and services to complete the transformation to a fully functional architecture.
