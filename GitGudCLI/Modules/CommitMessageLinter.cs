using ConsoleHelper;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GitGudCLI.Modules
{
    public class CommitMessageLinter
    {
        private readonly Regex _regex = new Regex(Constants.VALIDATION_REGEX, RegexOptions.Compiled | RegexOptions.Multiline);

        private EnumCommitError _errors;
        private EnumCommitWarning _warnings;

        public bool IsValid { get; private set; }
        public string CommitMessage { get; private set; }

        public bool HasTag { get; private set; }
        public string Tag { get; private set; }

        public bool HasFlags { get; private set; }
        public string[] Flags { get; private set; }

        public bool HasSubject { get; private set; }
        public string Subject { get; private set; }

        public bool HasBody { get; private set; }
        public string Body { get; private set; }

        public bool HasClosedIssues { get; private set; }
        public string[] ClosedIssues { get; private set; }

        public bool HasSeeAlso { get; private set; }
        public string[] SeeAlso { get; private set; }

        public CommitMessageLinter(string messageToValidate)
        {
            CommitMessage = messageToValidate;
            Validate(messageToValidate);
        }

        /// <summary>
        /// Validates a commit message
        /// </summary>
        /// <param name="commitMessage">The message to validate</param>
        /// <returns>If the validation was complete</returns>
        public bool Validate(string commitMessage)
        {
            var match = _regex.Match(commitMessage);

            // If the regex don't find anything, end here
            if (!match.Success) return false;

            // Se all the "Has" booleans
            HasTag = match.Groups["tag"].Success;
            HasFlags = match.Groups["flags"].Success;
            HasSubject = match.Groups["subject"].Success;
            HasBody = match.Groups["body"].Success;
            HasClosedIssues = match.Groups["closed_issues"].Success;
            HasSeeAlso = match.Groups["see_also"].Success;

            // The commit is already considered valid at this point if it has a tag and a subject
            IsValid = HasTag && HasSubject;

            // If it has no tag, raise the erro flag
            if (!HasTag) _errors |= EnumCommitError.NO_TAG;

            // If it has no subject, raise the error flag
            if (!HasSubject) _errors |= EnumCommitError.NO_SUBJECT;

            // If it has a tag, then remove the square brackets and save, if not save an empty string
            Tag = HasTag ? match.Groups["tag"].Value
                .Replace("[", string.Empty).Replace("]", string.Empty) : string.Empty;

            // If it has a subject, save it, if not save an empty string
            Subject = match.Groups["subject"].Value;

            // If it has a body, save it, if not save an empty string
            Body = match.Groups["body"].Value;

            // If it has an invalid tag, raise the error flag and invalidate the message
            if (HasTag && !Constants.VALID_COMMIT_TAGS.Contains(Tag))
            {
                _errors |= EnumCommitError.INVALID_TAG;
                IsValid = false;
            }

            // Try to get the flags
            try
            {
                // If it has flags, remove the first curly braces and split the string on the second ones, if not, set them to null
                Flags = HasFlags ? match.Groups["flags"].Value.Replace("{", string.Empty)
                            .Split('}', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) : null;

                // If we captured any flag
                if (Flags?.Length > 0)
                {
                    // Test each of the captured ones
                    foreach (string flag in Flags)
                    {
                        // If one flag isn't valid, raise the flag, invalidate the message and break the loop as we only need to raise the error flag once
                        if (!Constants.VALID_COMMIT_FLAGS.Contains(flag))
                        {
                            _errors |= EnumCommitError.INVALID_FLAG;
                            IsValid = false;
                            break;
                        }
                    }

                    // If the breaking change flag is present, but it's not the first flag to appear, then raise a warning flag
                    if (Flags.Contains("!!!") && Flags[0] != "!!!")
                        _warnings |= EnumCommitWarning.BREAKING_CHANGE_IS_NOT_THE_FIRST_FLAG;
                }
            }
            catch
            {
                // If the above fails, then one of the flags has a problem, raise the error flag and invalidate the message
                IsValid = false;
                _errors |= EnumCommitError.FLAG;
            }

            // Try to get the closed issues
            try
            {
                // If we a closed issues section, remove the "Closes: " line and split on the comma. if not set the array to null
                ClosedIssues = HasClosedIssues ? match.Groups["closed_issues"].Value.Replace("Closes: ", string.Empty)
                            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) : null;
            }
            catch
            {
                // If something above fails, invalidate the message and raise the error flag
                IsValid = false;
                _errors |= EnumCommitError.CLOSED_ISSUES;
            }

            // Try to get the see also section
            try
            {
                // If we a closed issues section, remove the "See also: " line and split on the comma. if not set the array to null
                SeeAlso = match.Groups["see_also"].Success ? match.Groups["see_also"].Value.Replace("See also: ", string.Empty)
                            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) : null;
            }
            catch
            {
                // If something above fails, invalidate the message and raise the error flag
                IsValid = false;
                _errors |= EnumCommitError.SEE_ALSO;
            }

            // If we have the subject but it's too long, raise an warning flag
            if (HasSubject && Subject.Length > 80)
                _warnings |= EnumCommitWarning.SUBJECT_TOO_LONG;

            // The validation was complete
            return true;
        }

        /// <summary>
        /// Writes a detailed, color coded report to the console
        /// </summary>
        public void WriteReport()
        {
            Console.OutputEncoding = Encoding.UTF8;

            byte errorsFound = 0;
            byte warningsFound = 0;

            if (!IsValid)
            {
                ColorConsole.WriteError("✘ The commit is not valid.");
            }
            else
            {
                ColorConsole.WriteSuccess("✓ The commit is valid.");
            }

            if (_errors != EnumCommitError.NONE)
            {
                if (_errors.HasFlag(EnumCommitError.NO_TAG))
                {
                    ColorConsole.WriteError("✘ The commit has no tag.");
                    errorsFound++;
                }

                if (_errors.HasFlag(EnumCommitError.NO_SUBJECT))
                {
                    ColorConsole.WriteError("✘ The commit has no subject.");
                    errorsFound++;
                }

                if (_errors.HasFlag(EnumCommitError.FLAG))
                {
                    ColorConsole.WriteError("✘ The flags are not valid.");
                    errorsFound++;
                }

                if (_errors.HasFlag(EnumCommitError.CLOSED_ISSUES))
                {
                    ColorConsole.WriteError("✘ The closed issues section is not valid.");
                    errorsFound++;
                }

                if (_errors.HasFlag(EnumCommitError.SEE_ALSO))
                {
                    ColorConsole.WriteError("✘ The see also section is not valid.");
                    errorsFound++;
                }

                if (_errors.HasFlag(EnumCommitError.INVALID_TAG))
                {
                    ColorConsole
                        .WriteError($"✘ The tag must be one of following: {string.Join(", ", Constants.VALID_COMMIT_TAGS)}.");
                    errorsFound++;
                }

                if (_errors.HasFlag(EnumCommitError.INVALID_FLAG))
                {
                    ColorConsole.WriteError($"✘ The tag must be one of following: {string.Join(", ", Constants.VALID_COMMIT_FLAGS)}.");
                    errorsFound++;
                }
            }

            if (_warnings != EnumCommitWarning.NONE)
            {
                if (_warnings.HasFlag(EnumCommitWarning.SUBJECT_TOO_LONG))
                {
                    ColorConsole.WriteWarning("⚠ The subject is too long, the maximum recommended length is 80 characters long.");
                    warningsFound++;
                }

                if (
                    _warnings.HasFlag(EnumCommitWarning.BREAKING_CHANGE_IS_NOT_THE_FIRST_FLAG)
                )
                {
                    ColorConsole.WriteWarning("⚠ The commit has the breaking-change flag {!!!} but it is not the first flag.");
                    warningsFound++;
                }
            }

            ColorConsole.WriteInfo($"🛈 Found {errorsFound} errors and {warningsFound} warnings.");
        }
    }
}