We welcome contributions! 

Here's how you can help:

## Description
<!-- Provide a clear and concise description of your changes. -->

## Related Issues
<!-- Link to related issues if applicable. -->
- Closes #ISSUE_ID
- Fixes #ISSUE_ID

## Type of Change
<!-- Mark with an "x" the types of changes that apply. -->
- [ ] 🐛 Bug fix (non-breaking change which fixes an issue)
- [ ] ✨ New feature (non-breaking change which adds functionality)
- [ ] 💥 Breaking change (fix or feature that would cause existing functionality to change)
- [ ] 🧹 Code cleanup / refactor
- [ ] 📖 Documentation update

## Checklist
<!-- Make sure you’ve completed the following before requesting a review. -->
- [ ] I have tested my changes locally against a Redmine instance.
- [ ] I have added/updated unit tests if applicable.
- [ ] I have updated documentation (README / XML comments / wiki).
- [ ] I followed the project’s coding style.
- [ ] New and existing tests pass with my changes.

## API Changes (if applicable)
<!-- Document any new or changed public methods/properties. -->
```csharp
// Example of a new/updated method signature
Task<Issue> GetByIdAsync(int id, bool includeChildren = false);
