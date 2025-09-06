const STORAGE_KEY = 'recipes-v1';
const API_BASE = '/api/recipe';

async function safeFetchJson(url, options) {
  try {
    const res = await fetch(url, options);
    if (!res.ok) {
      const text = await res.text();
      throw new Error(`${res.status} ${res.statusText} - ${text}`);
    }
    // No content
    if (res.status === 204) return null;
    const json = await res.json();
    return json;
  } catch (err) {
    // network or parse error — bubble up so callers decide, or return null
    console.warn('API request failed:', err);
    throw err;
  }
}

export async function loadAll() {
  // Try API first
  try {
    const data = await safeFetchJson(`${API_BASE}`);
    if (Array.isArray(data)) return data;
  } catch {
    // fallback to localStorage when API unavailable
  }

  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) return [];
  try {
    return JSON.parse(raw);
  } catch {
    return [];
  }
}

export async function findById(id) {
  try {
    const data = await safeFetchJson(`${API_BASE}/${id}`);
    if (data) return data;
  } catch {
    // fallback to local
  }

  const all = JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]');
  return all.find(r => r.id === id) || null;
}

export async function addRecipe(recipe) {
  const newRecipe = {
    id: recipe.id || undefined,
    title: recipe.title || 'Untitled',
    description: recipe.description || '',
    ingredients: recipe.ingredients || [],
    instructions: recipe.instructions || '',
    createdDate: recipe.createdDate || new Date().toISOString(),
  };

  // Try API first
  try {
    const created = await safeFetchJson(`${API_BASE}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newRecipe),
    });
    return created;
  } catch {
    // fallback to local
    const all = JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]');
    const id = newRecipe.id || cryptoRandomId();
    const localRecipe = { ...newRecipe, id };
    all.unshift(localRecipe);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(all));
    return localRecipe;
  }
}

export async function updateRecipe(id, patch) {
  // Get the existing recipe (prefer API)
  try {
    const existing = await safeFetchJson(`${API_BASE}/${id}`);
    if (!existing) throw new Error('Not found');
    const updated = { ...existing, ...patch, id };
    await safeFetchJson(`${API_BASE}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updated),
    });
    return updated;
  } catch {
    // fallback to local
    const all = JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]');
    const i = all.findIndex(r => r.id === id);
    if (i === -1) return null;
    all[i] = { ...all[i], ...patch };
    localStorage.setItem(STORAGE_KEY, JSON.stringify(all));
    return all[i];
  }
}

export async function removeRecipe(id) {
  try {
    await safeFetchJson(`${API_BASE}/${id}`, { method: 'DELETE' });
    return true;
  } catch {
    // fallback to local
    const all = JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]').filter(r => r.id !== id);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(all));
    return true;
  }
}

/* Helper for fallback local IDs */
function cryptoRandomId() {
  // RFC4122-like fallback using random numbers
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}