﻿// Copyright (c) 2007-2011 SlimDX Group
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Runtime.InteropServices;

namespace SlimDX.Generator
{
	class DirectFormatter : Formatter
	{
		#region Interface

		/// <summary>
		/// Gets the code for declaring the specified model as a parameter to a managed method.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>The code.</returns>
		public string GetFormalParameterCode(ParameterModel model)
		{
			if (model.Flags.HasFlag(ParameterModelFlags.IsOutput))
				return string.Format("out {0} {1}", model.Type.Name, model.Name);
			else
				return string.Format("{0} {1}", model.Type.Name, model.Name);
		}

		/// <summary>
		/// Gets the code for passing the specified model as parameter to a trampoline method.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>The code.</returns>
		public string GetTrampolineParameterCode(ParameterModel model)
		{
			if (model.Flags.HasFlag(ParameterModelFlags.IsOutput))
				return string.Format("ref _{0}", model.Name);

			TranslationModel translationModel = model.Type as TranslationModel;
			if (translationModel != null)
			{
				var type = Type.GetType(translationModel.TargetType);
				if (Marshal.SizeOf(type) > sizeof(long))
					return string.Format("new System.IntPtr(&{0})", model.Name);
			}

			return model.Name;
		}

		/// <summary>
		/// Gets the code for setup of local variables related to the specified parameter.
		/// </summary>
		/// <param name="marshaller">The marshalling service interface.</param>
		/// <param name="model">The model.</param>
		/// <returns>The code.</returns>
		public string GetLocalVariableSetupCode(MarshallingService marshaller, ParameterModel model)
		{
			if (model.Flags.HasFlag(ParameterModelFlags.IsOutput))
				return string.Format("System.IntPtr _{0} = default(System.IntPtr);", model.Name);
			return string.Empty;
		}

		/// <summary>
		/// Gets the code for cleanup of local variables related to the specified parameter.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>The code.</returns>
		public string GetLocalVariableCleanupCode(ParameterModel model)
		{
			if (model.Flags.HasFlag(ParameterModelFlags.IsOutput))
				return string.Format("{0} = _{0};", model.Name);
			return string.Empty;
		}

		#endregion
	}
}
