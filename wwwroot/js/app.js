window.dartCounter = {
    applyTheme: function(theme, accent) {
        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.style.setProperty('--accent', accent);
        var meta = document.querySelector('meta[name=theme-color]');
        if (meta) meta.setAttribute('content', theme === 'dark' ? '#0f1115' : '#f4f6fb');
    },
    playClick: function(volume) {
        try {
            var ctx = window._dartAudioCtx = window._dartAudioCtx || new (window.AudioContext || window.webkitAudioContext)();
            var osc = ctx.createOscillator();
            var gain = ctx.createGain();
            osc.connect(gain);
            gain.connect(ctx.destination);
            osc.frequency.value = 800;
            osc.type = 'square';
            gain.gain.setValueAtTime((volume || 0.5) * 0.1, ctx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.05);
            osc.start();
            osc.stop(ctx.currentTime + 0.05);
        } catch(e) {}
    },
    playSfx: function(sfx, volume) {
        try {
            var ctx = window._dartAudioCtx = window._dartAudioCtx || new (window.AudioContext || window.webkitAudioContext)();
            var osc = ctx.createOscillator();
            var gain = ctx.createGain();
            osc.connect(gain);
            gain.connect(ctx.destination);
            var freq = 440;
            var dur = 0.1;
            switch(sfx) {
                case 'hit': freq = 600; dur = 0.08; break;
                case 'bust': freq = 150; dur = 0.3; break;
                case 'victory': freq = 880; dur = 0.5; break;
                case 'showdown': freq = 200; dur = 0.4; break;
                case 'milestone': freq = 660; dur = 0.2; break;
                case 'levelup': freq = 523; dur = 0.3; break;
                case 'kill': freq = 100; dur = 0.2; break;
                default: freq = 440; dur = 0.1;
            }
            osc.frequency.value = freq;
            osc.type = 'sine';
            gain.gain.setValueAtTime((volume || 0.5) * 0.15, ctx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + dur);
            osc.start();
            osc.stop(ctx.currentTime + dur);
        } catch(e) {}
    }
};
