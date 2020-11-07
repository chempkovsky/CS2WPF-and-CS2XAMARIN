using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2WPF.TemplateProcessingHelpers
{
	/// <summary>
	/// Contains information about result of running a
	/// visual studio T4 text template transformation.
	/// </summary>
	public class TPResult
	{
		/// <summary>
		/// Generated text of transformation if it was
		/// successful. This will be empty when there are processing errors.
		/// </summary>
		public string GeneratedText
		{
			get;
			set;
		}

		/// <summary>
		/// Any errors resulted from transformation.
		/// </summary>
		public IReadOnlyCollection<TPError> ProcessingErrors
		{
			get;
			set;
		}

		/// <summary>
		/// The file extension specified by T4 text template.
		/// </summary>
		public string TemplateFileExtension
		{
			get;
			set;
		}
	}

}
