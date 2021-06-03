using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GitGudCLI.Structure;
using GitGudCLI.Utils;

namespace GitGudCLI.Modules
{
	public class CommitMessageLinter
	{
		private readonly Regex _headerRegex = new(Constants.HeaderValidationRegex, RegexOptions.Compiled);
		private readonly Regex _footerRegex = new(Constants.FooterValidationRegex, RegexOptions.Compiled);

		private EnumCommitError _errors;
		private EnumCommitWarning _warnings;

		public CommitMessageLinter(string messageToValidate)
		{
			CommitMessage = messageToValidate;
			Validate(messageToValidate);
		}

		public bool IsValid { get; private set; }
		public string CommitMessage { get; }

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

        /// <summary>
        /// Validates a commit message
        /// </summary>
        /// <param name="commitMessage">The message to validate</param>
        /// <returns>If the validation was complete</returns>
        private bool Validate(string commitMessage)
        {
	        string[] messageSplit = commitMessage.Split("~~~",
		        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	        
			var header = _headerRegex.Match(messageSplit[0]);
			var footer = messageSplit.Length switch
			{
				1 => null,
				2 => _footerRegex.Match(messageSplit[1]),
				3 => _footerRegex.Match(messageSplit[2]),
				_ => null
			};
			
			// If the regex don't find a header, end here
			if (!header.Success) return false;

			// Se all the "Has" booleans
			HasTag = header.Groups["tag"].Success;
			HasFlags = header.Groups["flags"].Success;
			HasSubject = header.Groups["subject"].Success;

			HasBody = messageSplit.Length switch
			{
				1 => false,
				2 => !footer?.Success ?? false,
				3 => !string.IsNullOrWhiteSpace(messageSplit[1]),
				_ => false
			};
			
			HasClosedIssues = footer?.Groups["closed_issues"].Success ?? false;
			HasSeeAlso = footer?.Groups["see_also"].Success ?? false;

			// The commit is already considered valid at this point if it has a tag and a subject
			IsValid = HasTag && HasSubject;

			// If it has no tag, raise the erro flag
			if (!HasTag) _errors |= EnumCommitError.NO_TAG;

			// If it has no subject, raise the error flag
			if (!HasSubject) _errors |= EnumCommitError.NO_SUBJECT;

			// If it has a tag, then remove the square brackets and save, if not save an empty string
			Tag = HasTag
				? header.Groups["tag"].Value
					.Replace("[", string.Empty).Replace("]", string.Empty)
				: string.Empty;

			// If it has a subject, save it, if not save an empty string
			Subject = HasSubject ? header.Groups["subject"].Value : string.Empty;
			
			// If it has a body, save it, if not save an empty string
			Body =  HasBody ? messageSplit[1] : string.Empty;

			// If it has an invalid tag, raise the error flag and invalidate the message
			if (HasTag && !Constants.ValidCommitTags.Contains(Tag))
			{
				_errors |= EnumCommitError.INVALID_TAG;
				IsValid = false;
			}

			// Try to get the flags
			try
			{
				// If it has flags, remove the first curly braces and split the string on the second ones, if not, set them to null
				Flags = HasFlags
					? header.Groups["flags"].Value.Replace("{", string.Empty).Replace("}", string.Empty)
						.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
					: null;

				// If we captured any flag
				if (Flags?.Length > 0)
				{
					// Test each of the captured ones
					if (Flags.Any(flag => !Constants.ValidCommitFlags.Contains(flag)))
					{
						_errors |= EnumCommitError.INVALID_FLAG;
						IsValid = false;
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
				ClosedIssues = HasClosedIssues
					? footer.Groups["closed_issues"].Value.Replace("Closes: ", string.Empty)
						.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
					: null;
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
				SeeAlso = HasSeeAlso
					? footer.Groups["see_also"].Value.Replace("See also: ", string.Empty)
						.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
					: null;
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
        ///     Writes a detailed, color coded report to the console
        /// </summary>
        public void WriteReport()
		{
			Console.OutputEncoding = Encoding.UTF8;

			byte errorsFound = 0;
			byte warningsFound = 0;

			if (!IsValid)
				SpectreHelper.WriteError("✘ The commit is not valid.");
			else
				SpectreHelper.WriteSuccess("✓ The commit is valid.");

			if (_errors != EnumCommitError.NONE)
			{
				if (_errors.HasFlag(EnumCommitError.NO_TAG))
				{
					SpectreHelper.WriteError("✘ The commit has no tag.");
					errorsFound++;
				}

				if (_errors.HasFlag(EnumCommitError.NO_SUBJECT))
				{
					SpectreHelper.WriteError("✘ The commit has no subject.");
					errorsFound++;
				}

				if (_errors.HasFlag(EnumCommitError.FLAG))
				{
					SpectreHelper.WriteError("✘ The flags are not valid.");
					errorsFound++;
				}

				if (_errors.HasFlag(EnumCommitError.CLOSED_ISSUES))
				{
					SpectreHelper.WriteError("✘ The closed issues section is not valid.");
					errorsFound++;
				}

				if (_errors.HasFlag(EnumCommitError.SEE_ALSO))
				{
					SpectreHelper.WriteError("✘ The see also section is not valid.");
					errorsFound++;
				}

				if (_errors.HasFlag(EnumCommitError.INVALID_TAG))
				{
					SpectreHelper
						.WriteError(
							$"✘ The tag must be one of following: {string.Join(", ", Constants.ValidCommitTags)}.");
					errorsFound++;
				}

				if (_errors.HasFlag(EnumCommitError.INVALID_FLAG))
				{
					SpectreHelper.WriteError(
						$"✘ The tag must be one of following: {string.Join(", ", Constants.ValidCommitFlags)}.");
					errorsFound++;
				}
			}

			if (_warnings != EnumCommitWarning.NONE)
			{
				if (_warnings.HasFlag(EnumCommitWarning.SUBJECT_TOO_LONG))
				{
					SpectreHelper.WriteWarning(
						"⚠ The subject is too long, the maximum recommended length is 80 characters long.");
					warningsFound++;
				}

				if (
					_warnings.HasFlag(EnumCommitWarning.BREAKING_CHANGE_IS_NOT_THE_FIRST_FLAG)
				)
				{
					SpectreHelper.WriteWarning(
						"⚠ The commit has the breaking-change flag {!!!} but it is not the first flag.");
					warningsFound++;
				}
			}

			SpectreHelper.WriteInfo($"🛈 Found {errorsFound} errors and {warningsFound} warnings.");
		}
	}
}