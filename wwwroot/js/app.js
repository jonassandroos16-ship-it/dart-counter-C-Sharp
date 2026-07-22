window.dartCounter = {
  audioCtx: null,
  musicNode: null,

  initSupabase(url, key) {
    if (url) localStorage.setItem('dc_supabase_url', url);
    if (key) localStorage.setItem('dc_supabase_anon_key', key);
  },

  getCtx() {
    if (!this.audioCtx) {
      this.audioCtx = new (window.AudioContext || window.webkitAudioContext)();
    }
    return this.audioCtx;
  },

  playClick(clickSound = 'tick', vol = 0.6) {
    try {
      const ctx = this.getCtx();
      const t = ctx.currentTime;
      const osc = ctx.createOscillator();
      const gain = ctx.createGain();
      if (clickSound === 'pop') {
        osc.frequency.setValueAtTime(800, t);
        osc.frequency.exponentialRampToValueAtTime(400, t + 0.05);
      } else if (clickSound === 'tap') {
        osc.frequency.setValueAtTime(200, t);
      } else {
        osc.frequency.setValueAtTime(1000, t);
      }
      osc.type = clickSound === 'tick' ? 'square' : 'sine';
      gain.gain.setValueAtTime(vol * 0.3, t);
      gain.gain.exponentialRampToValueAtTime(0.001, t + 0.05);
      osc.connect(gain).connect(ctx.destination);
      osc.start(t);
      osc.stop(t + 0.05);
    } catch(e) {}
  },

  playHit(pack = 'thud', vol = 0.9, score = 0) {
    try {
      const ctx = this.getCtx();
      const t = ctx.currentTime;
      const osc = ctx.createOscillator();
      const gain = ctx.createGain();
      const freq = pack === 'arcade' ? 440 + score * 2 : pack === 'punch' ? 150 + score : pack === 'board' ? 300 : 200;
      osc.frequency.setValueAtTime(freq, t);
      osc.frequency.exponentialRampToValueAtTime(freq * 0.5, t + 0.1);
      osc.type = pack === 'arcade' ? 'square' : 'sine';
      gain.gain.setValueAtTime(vol * 0.4, t);
      gain.gain.exponentialRampToValueAtTime(0.001, t + 0.15);
      osc.connect(gain).connect(ctx.destination);
      osc.start(t);
      osc.stop(t + 0.15);
    } catch(e) {}
  },

  playSfx(sfx, vol = 0.9) {
    try {
      const ctx = this.getCtx();
      const t = ctx.currentTime;
      const playTone = (freq, start, dur, type = 'sine', volMul = 1) => {
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.frequency.setValueAtTime(freq, t + start);
        osc.type = type;
        gain.gain.setValueAtTime(vol * 0.3 * volMul, t + start);
        gain.gain.exponentialRampToValueAtTime(0.001, t + start + dur);
        osc.connect(gain).connect(ctx.destination);
        osc.start(t + start);
        osc.stop(t + start + dur);
      };
      if (sfx === 'hit') playTone(300, 0, 0.1);
      else if (sfx === 'bust') { playTone(200, 0, 0.15, 'sawtooth'); playTone(150, 0.08, 0.15, 'sawtooth'); }
      else if (sfx === 'victory') { playTone(523, 0, 0.15); playTone(659, 0.12, 0.15); playTone(784, 0.24, 0.3); }
      else if (sfx === 'showdown') { playTone(440, 0, 0.1, 'square'); playTone(660, 0.1, 0.1, 'square'); playTone(880, 0.2, 0.2, 'square'); }
      else if (sfx === 'showdown_close') playTone(400, 0, 0.1, 'sine');
      else if (sfx === 'milestone') { playTone(523, 0, 0.1); playTone(659, 0.08, 0.1); playTone(784, 0.16, 0.2); }
      else if (sfx === 'levelup') { playTone(523, 0, 0.1); playTone(659, 0.08, 0.1); playTone(784, 0.16, 0.1); playTone(1047, 0.24, 0.3); }
      else if (sfx === 'title') { playTone(659, 0, 0.15); playTone(880, 0.12, 0.15); playTone(1047, 0.24, 0.3); }
      else if (sfx === 'kill') { playTone(200, 0, 0.1, 'sawtooth'); playTone(100, 0.08, 0.2, 'sawtooth'); }
      else if (sfx === 'powerup') { playTone(440, 0, 0.08); playTone(550, 0.06, 0.08); playTone(660, 0.12, 0.15); }
      else playTone(440, 0, 0.1);
    } catch(e) {}
  },

  playEntrance(name, vol = 0.9) {
    try {
      const ctx = this.getCtx();
      const t = ctx.currentTime;
      const themes = {
        Hero: [523, 659, 784],
        Villain: [200, 150, 100],
        Cyborg: [440, 880, 440],
        Mystic: [330, 440, 550],
        Beast: [150, 100, 200],
        Champion: [784, 988, 1047, 1319],
      };
      const notes = themes[name] || [440];
      notes.forEach((f, i) => {
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.frequency.setValueAtTime(f, t + i * 0.1);
        osc.type = 'sine';
        gain.gain.setValueAtTime(vol * 0.3, t + i * 0.1);
        gain.gain.exponentialRampToValueAtTime(0.001, t + i * 0.1 + 0.15);
        osc.connect(gain).connect(ctx.destination);
        osc.start(t + i * 0.1);
        osc.stop(t + i * 0.1 + 0.15);
      });
    } catch(e) {}
  },

  startMusic(track, vol = 0.9) {
    this.stopMusic();
    try {
      const ctx = this.getCtx();
      const t = ctx.currentTime;
      const bassNotes = [130, 165, 196, 165];
      let beatTime = t;
      const playBeat = () => {
        if (!this.musicNode) return;
        const note = bassNotes[Math.floor((beatTime - t) / 0.5) % bassNotes.length];
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.frequency.setValueAtTime(note, beatTime);
        osc.type = 'triangle';
        gain.gain.setValueAtTime(vol * 0.15, beatTime);
        gain.gain.exponentialRampToValueAtTime(0.001, beatTime + 0.4);
        osc.connect(gain).connect(ctx.destination);
        osc.start(beatTime);
        osc.stop(beatTime + 0.4);
        beatTime += 0.5;
      };
      this.musicNode = setInterval(playBeat, 500);
    } catch(e) {}
  },

  stopMusic() {
    if (this.musicNode) {
      clearInterval(this.musicNode);
      this.musicNode = null;
    }
  },

  getDeviceId() {
    let id = localStorage.getItem('dc_device_id');
    if (!id) {
      id = Math.random().toString(36).slice(2, 12);
      localStorage.setItem('dc_device_id', id);
    }
    return id;
  },

  async mpCreateLobby(lobby) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return;
      const sb = createClient(url, key);
      await sb.from('mp_lobbies').insert({
        id: lobby.id, code: lobby.code, name: lobby.name,
        host_device_id: lobby.hostDeviceId, host_player_id: lobby.hostPlayerId,
        status: lobby.status, created_at: lobby.createdAt, updated_at: lobby.updatedAt,
      });
    } catch(e) { console.warn('[mp] createLobby:', e.message); }
  },

  async mpJoinLobby(lobbyId, player, deviceId) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return;
      const sb = createClient(url, key);
      await sb.from('mp_lobby_players').insert({
        lobby_id: lobbyId, device_id: deviceId,
        player_id: player.id, player_name: player.name,
        player_color: player.color, ready: false,
      });
    } catch(e) { console.warn('[mp] joinLobby:', e.message); }
  },

  async mpLeaveLobby(lobbyId, deviceId, playerId) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return;
      const sb = createClient(url, key);
      await sb.from('mp_lobby_players').delete()
        .eq('lobby_id', lobbyId).eq('device_id', deviceId).eq('player_id', playerId);
    } catch(e) {}
  },

  async mpDeleteLobby(lobbyId) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return;
      const sb = createClient(url, key);
      await sb.from('mp_lobbies').delete().eq('id', lobbyId);
    } catch(e) {}
  },

  async mpSetReady(lobbyId, deviceId, playerId, ready) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return;
      const sb = createClient(url, key);
      await sb.from('mp_lobby_players').update({ ready })
        .eq('lobby_id', lobbyId).eq('device_id', deviceId).eq('player_id', playerId);
    } catch(e) {}
  },

  async mpFetchLobby(lobbyId) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return null;
      const sb = createClient(url, key);
      const { data } = await sb.from('mp_lobbies').select('*').eq('id', lobbyId).maybeSingle();
      return data;
    } catch(e) { return null; }
  },

  async mpFetchPlayers(lobbyId) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return [];
      const sb = createClient(url, key);
      const { data } = await sb.from('mp_lobby_players').select('*')
        .eq('lobby_id', lobbyId).order('joined_at', { ascending: true });
      return data || [];
    } catch(e) { return []; }
  },

  async mpFetchOpenLobbies() {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return [];
      const sb = createClient(url, key);
      const { data } = await sb.from('mp_lobbies').select('*')
        .eq('status', 'lobby').order('created_at', { ascending: false }).limit(50);
      return data || [];
    } catch(e) { return []; }
  },

  async mpFetchByCode(code) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return null;
      const sb = createClient(url, key);
      const { data } = await sb.from('mp_lobbies').select('*')
        .eq('code', code.toUpperCase()).eq('status', 'lobby').maybeSingle();
      return data;
    } catch(e) { return null; }
  },

  async mpUpdateGameState(lobbyId, game) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return;
      const sb = createClient(url, key);
      await sb.from('mp_lobbies').update({
        game_state: game, player_turn: game.currentIdx, updated_at: new Date().toISOString(),
      }).eq('id', lobbyId);
    } catch(e) {}
  },

  async mpStartGame(lobbyId, config, game) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return;
      const sb = createClient(url, key);
      await sb.from('mp_lobbies').update({
        status: 'playing', game_config: config, game_state: game,
        player_turn: game.currentIdx, updated_at: new Date().toISOString(),
      }).eq('id', lobbyId);
    } catch(e) {}
  },

  async mpSetStatus(lobbyId, status) {
    try {
      const { createClient } = await import('https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2');
      const url = localStorage.getItem('dc_supabase_url') || '';
      const key = localStorage.getItem('dc_supabase_anon_key') || '';
      if (!url || !key) return;
      const sb = createClient(url, key);
      await sb.from('mp_lobbies').update({ status, updated_at: new Date().toISOString() }).eq('id', lobbyId);
    } catch(e) {}
  },

  drawDartboardHeatmap(canvasId, hitData) {
    try {
      const canvas = document.getElementById(canvasId);
      if (!canvas) return;
      const ctx = canvas.getContext('2d');
      const dpr = window.devicePixelRatio || 1;
      const rect = canvas.getBoundingClientRect();
      canvas.width = rect.width * dpr;
      canvas.height = rect.width * dpr;
      ctx.scale(dpr, dpr);
      const size = rect.width;
      const cx = size / 2, cy = size / 2;
      const r = size / 2 - 4;
      const numbers = [20,1,18,4,13,6,10,15,2,17,3,19,7,16,8,11,14,9,12,5];
      ctx.clearRect(0, 0, size, size);
      for (let i = 0; i < 20; i++) {
        const a1 = (i * 18 - 99) * Math.PI / 180;
        const a2 = ((i + 1) * 18 - 99) * Math.PI / 180;
        const hits = hitData[numbers[i]] || 0;
        const maxHits = Math.max(1, ...Object.values(hitData));
        const intensity = hits / maxHits;
        ctx.beginPath();
        ctx.moveTo(cx, cy);
        ctx.arc(cx, cy, r, a1, a2);
        ctx.closePath();
        if (intensity > 0) {
          ctx.fillStyle = `rgba(34, 197, 94, ${0.15 + intensity * 0.65})`;
        } else {
          ctx.fillStyle = 'rgba(31, 36, 46, 0.5)';
        }
        ctx.fill();
        ctx.strokeStyle = 'rgba(42, 48, 60, 0.8)';
        ctx.lineWidth = 1;
        ctx.stroke();
      }
      ctx.beginPath();
      ctx.arc(cx, cy, r * 0.15, 0, Math.PI * 2);
      ctx.fillStyle = hitData[25] || hitData[50] ? 'rgba(34, 197, 94, 0.5)' : 'rgba(31, 36, 46, 0.5)';
      ctx.fill();
      ctx.strokeStyle = 'rgba(42, 48, 60, 0.8)';
      ctx.stroke();
    } catch(e) {}
  },

  drawLineChart(canvasId, data, color) {
    try {
      const canvas = document.getElementById(canvasId);
      if (!canvas) return;
      const ctx = canvas.getContext('2d');
      const dpr = window.devicePixelRatio || 1;
      const rect = canvas.getBoundingClientRect();
      canvas.width = rect.width * dpr;
      canvas.height = 160 * dpr;
      ctx.scale(dpr, dpr);
      const w = rect.width, h = 160;
      ctx.clearRect(0, 0, w, h);
      if (!data || data.length === 0) return;
      const max = Math.max(...data, 1);
      const min = Math.min(...data, 0);
      const range = max - min || 1;
      ctx.beginPath();
      data.forEach((v, i) => {
        const x = (i / Math.max(1, data.length - 1)) * (w - 20) + 10;
        const y = h - 10 - ((v - min) / range) * (h - 20);
        if (i === 0) ctx.moveTo(x, y);
        else ctx.lineTo(x, y);
      });
      ctx.strokeStyle = color || '#22c55e';
      ctx.lineWidth = 2;
      ctx.stroke();
    } catch(e) {}
  },

  drawBarChart(canvasId, labels, values, color) {
    try {
      const canvas = document.getElementById(canvasId);
      if (!canvas) return;
      const ctx = canvas.getContext('2d');
      const dpr = window.devicePixelRatio || 1;
      const rect = canvas.getBoundingClientRect();
      canvas.width = rect.width * dpr;
      canvas.height = 160 * dpr;
      ctx.scale(dpr, dpr);
      const w = rect.width, h = 160;
      ctx.clearRect(0, 0, w, h);
      if (!values || values.length === 0) return;
      const max = Math.max(...values, 1);
      const barW = (w - 20) / values.length;
      values.forEach((v, i) => {
        const bh = (v / max) * (h - 30);
        const x = 10 + i * barW;
        const y = h - 10 - bh;
        ctx.fillStyle = color || '#22c55e';
        ctx.fillRect(x + 2, y, barW - 4, bh);
        ctx.fillStyle = 'rgba(139, 148, 167, 0.8)';
        ctx.font = '10px sans-serif';
        ctx.textAlign = 'center';
        ctx.fillText(labels[i], x + barW / 2, h - 2);
      });
    } catch(e) {}
  },
};
