export const initialize = async () => {   
    document.addEventListener('keydown', keydownHandler);
}

export const dispose = () => {
    document.removeEventListener('keydown', keydownHandler);
}

let dotNet;

const keydownHandler = async (event) => {
    if (!dotNet) {
        const { getAssemblyExports } = await globalThis.getDotnetRuntime(0);
        var exports = await getAssemblyExports("OptionA.Composer.dll");
        dotNet = exports.OptionA.Composer.Components.Sheet.OptAMusicSheet;
    }

    var toSend = {
        altKey: event.altKey,
        ctrlKey: event.ctrlKey,
        keyCode: event.keyCode,
        shiftKey: event.shiftKey,
        key: event.key 
    }

    console.log(toSend);
    const eventJson = JSON.stringify(toSend);
    console.log(eventJson);
    dotNet.KeyDown(eventJson);
}