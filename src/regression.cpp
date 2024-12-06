#include <vector>
#include <string>

// With USE_SDK it should be 2 EntryPoints
// Without 0

#if USE_SDK // get slang from the vulkan SDK
#include <slang/slang.h>
#include <slang/slang-gfx.h> 
#else
#include <slang.h>
#include <slang-gfx.h>
#endif

#include <iostream>
#include <filesystem>

using namespace slang;
using namespace gfx;

int main()
{
	std::filesystem::path shader = std::filesystem::path(__FILE__).parent_path() / "shader.slang";

	ComPtr<IGlobalSession> globalSession;

	createGlobalSession(globalSession.writeRef());

	std::vector<CompilerOptionEntry> compilerOptions;
	compilerOptions.emplace_back(
		CompilerOptionEntry{
			CompilerOptionName::VulkanUseEntryPointName,
			CompilerOptionValue {.kind = CompilerOptionValueKind::Int, .intValue0 = 1 }
		});

	SessionDesc desc = {};
	desc.flags = kSessionFlags_None;
	desc.defaultMatrixLayoutMode = SLANG_MATRIX_LAYOUT_ROW_MAJOR;
	desc.fileSystem = nullptr;
	desc.allowGLSLSyntax = false;
	desc.enableEffectAnnotations = false;
	desc.compilerOptionEntries = compilerOptions.data();
	desc.compilerOptionEntryCount = (uint32_t)compilerOptions.size();

	TargetDesc targetDesc = {};
	targetDesc.format = SLANG_SPIRV;
	targetDesc.profile = globalSession->findProfile("spirv_1_6");

	desc.targets = &targetDesc;
	desc.targetCount = 1;

	ComPtr<ISession> session;
	globalSession->createSession(desc, session.writeRef());


	ComPtr<IBlob> diagnostics;
	IModule* module = session->loadModule(shader.string().c_str(), diagnostics.writeRef());

	if (diagnostics)
	{
		std::cout << "Diagnostics: " << (const char*)diagnostics->getBufferPointer() << std::endl;
	}

	if (!module)
	{
		std::cout << "Failed to Load/Compile shader: " << shader.string() << std::endl;

		return false;
	}

	std::vector<ComPtr<IEntryPoint>> entryPoints;

	for (SlangInt32 i = 0; i < module->getDefinedEntryPointCount(); i++)
	{
		ComPtr<IEntryPoint> entry;
		module->getDefinedEntryPoint(i, entry.writeRef());

		if (entry)
		{
			entryPoints.push_back(entry);
		}
	}

	std::vector<IComponentType*> componentTypes;
	componentTypes.push_back(module);

	for (auto& entry : entryPoints)
	{
		componentTypes.push_back(entry);
	}

	ComPtr<IComponentType> composedProgram;
	ComPtr<ISlangBlob> diagnosticsBlob;
	SlangResult result = session->createCompositeComponentType(
		componentTypes.data(),
		(SlangInt)componentTypes.size(),
		composedProgram.writeRef(),
		diagnosticsBlob.writeRef());

	if (diagnosticsBlob)
	{
		
	}

	if (SLANG_FAILED(result))
	{
		if (diagnosticsBlob)
		{
			std::cout << "Diagnostics: " << (const char*)diagnosticsBlob->getBufferPointer() << std::endl;
		}

		return false;
	}

	ProgramLayout* slangReflection = composedProgram->getLayout();

	IShaderProgram::Desc programDesc = {};
	programDesc.slangGlobalScope = composedProgram.get();

	ComPtr<IComponentType> linkedProgram;
	ComPtr<ISlangBlob> linkDiagnosticsBlob;
	SlangResult linkResult = composedProgram->link(linkedProgram.writeRef(), linkDiagnosticsBlob.writeRef());

	if (linkDiagnosticsBlob)
	{
		std::cout << "Diagnostics: " << (const char*)linkDiagnosticsBlob->getBufferPointer() << std::endl;
	}

	if (SLANG_FAILED(linkResult))
	{
		std::cout << "Failed to link shader program";

		return false;
	}

	ProgramLayout* linkedProgramLayout = linkedProgram->getLayout();

	uint32_t entryPointCount = linkedProgramLayout->getEntryPointCount();

	std::cout << "Entry point count: " << entryPointCount;

	return 0;
}