---
name: unity_testing
description: Unity Testing Standards. Implements Edit/Play mode tests using AAA pattern and NUnit model.
---

# Testing Standards

## Edit Mode Tests
Tests logic independent of Unity loop. Place in `Tests/EditMode/`.

## Play Mode Tests
Tests GameObjects/Physics. Place in `Tests/PlayMode/`.

## AAA Pattern & Assert.That
```csharp
[Test]
public void Calc_Sum_ReturnsCorrect() {
    // Arrange
    int a = 2, b = 3;
    // Act
    int res = a + b;
    // Assert
    Assert.That(res, Is.EqualTo(5));
}
```
