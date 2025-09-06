const STORAGE_KEY = 'recipes-v1';

function uid() {
  // small UUID v4-like generator
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

export function loadAll() {
  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) return [];
  try {
    return JSON.parse(raw);
  } catch {
    return [];
  }
}

export function saveAll(list) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(list));
}

export async function addRecipe(recipe) {
  const all = loadAll();
  const newRecipe = {
    id: uid(),
    title: recipe.title || 'Untitled',
    description: recipe.description || '',
    ingredients: recipe.ingredients || [],
    instructions: recipe.instructions || '',
    createdDate: new Date().toISOString(),
  };
  all.unshift(newRecipe);
  saveAll(all);

  // Try to POST to the API but don't fail the UI if the server is unreachable
  try {
    await fetch('/api/recipe', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        id: newRecipe.id,
        title: newRecipe.title,
        description: newRecipe.description,
        ingredients: newRecipe.ingredients,
        instructions: newRecipe.instructions,
        createdDate: newRecipe.createdDate,
      }),
    });
  } catch {
    // ignore network errors
  }

  return newRecipe;
}

export function updateRecipe(id, patch) {
  const all = loadAll();
  const i = all.findIndex(r => r.id === id);
  if (i === -1) return null;
  all[i] = { ...all[i], ...patch };
  saveAll(all);
  return all[i];
}

export function removeRecipe(id) {
  let all = loadAll();
  all = all.filter(r => r.id !== id);
  saveAll(all);
}

export function findById(id) {
  const all = loadAll();
  return all.find(r => r.id === id) || null;
}