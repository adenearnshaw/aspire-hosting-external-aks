using Hex1b;
using Hex1b.Widgets;

namespace Aspire.Hosting.ExternalAks.Console;

public static class MainView
{
	internal static Func<RootContext, AppState, VStackWidget> Build = (ctx, state) =>
	{
		return ctx.VStack(v =>
		{
			var contentWidgets = new List<Hex1bWidget>
			{
				v.Text("📋 AKS External Service Connection Test"),
				v.Text(""),
				v.HStack(h => [
					h.Text("Connection String: "),
				h.Text(state.ConnectionString ?? "NOT CONFIGURED")
				]),
				v.Text(""),
				new SeparatorWidget(),
				v.Text("")
			};

			if (state.IsLoading)
			{
				contentWidgets.Add(v.Text("⏳ Testing connection..."));
			}
			else if (state.Error != null)
			{
				contentWidgets.Add(v.Text($"❌ Error: {state.Error}"));
				contentWidgets.Add(v.Text(""));
				contentWidgets.Add(v.Button("Retry").OnClick(_ => state.TestConnection()));
				contentWidgets.Add(v.Button("Exit").OnClick(_ => Environment.Exit(state.ExitCode)));
			}
			else
			{
				contentWidgets.Add(v.Text($"✅ Status Code: {state.StatusCode}"));
				contentWidgets.Add(v.Text($"🔗 Endpoint: {state.Endpoint}"));
				contentWidgets.Add(v.Text(""));
				contentWidgets.Add(v.Text("Response:"));
				contentWidgets.Add(v.Text(state.Response ?? ""));
				contentWidgets.Add(v.Text(""));
				contentWidgets.Add(v.Button("Exit").OnClick(_ => Environment.Exit(0)));
			}

			return [
				v.Border(_ => contentWidgets.ToArray()).Fill(),
				v.InfoBar("Tab: Focus  Enter: Activate  Ctrl+C: Exit")
			];
		});
	};
}