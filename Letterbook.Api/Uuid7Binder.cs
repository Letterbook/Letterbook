using Letterbook.Core.Extensions;
using Medo;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Letterbook.Api;

public class Uuid7Binder : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		ArgumentNullException.ThrowIfNull(bindingContext);

		var modelName = bindingContext.ModelName;
		var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
		var value = valueProviderResult.FirstValue;
		if (string.IsNullOrEmpty(value))
		{
			return Task.CompletedTask;
		}

		if (!Id.TryAsUuid7(value, out var uuid))
		{
			bindingContext.ModelState.TryAddModelError(
				modelName, "value is not an Id25 string");

			return Task.CompletedTask;
		}

		bindingContext.Result = ModelBindingResult.Success(uuid);
		return Task.CompletedTask;
	}
}

public class Uuid7BinderProvider : IModelBinderProvider
{
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		return context.Metadata.ModelType == typeof(Uuid7)
			? new BinderTypeModelBinder(typeof(Uuid7Binder))
			: default(IModelBinder?);
	}
}