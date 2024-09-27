export const initialize = async () => {   
    document.addEventListener('keydown', keydownHandler);
}

export const dispose = () => {
    document.removeEventListener('keydown', keydownHandler);
}

let dotNet;

const keydownHandler = async (event) => {
    console.log(event);
    if (!dotNet) {
        const { getAssemblyExports } = await globalThis.getDotnetRuntime(0);
        var exports = await getAssemblyExports("OptionA.Composer.dll");
        dotNet = exports.OptionA.Composer.Components.Sheet.OptAMusicSheet;
    }

    dotNet.KeyDown(event.key);
}