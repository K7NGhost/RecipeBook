import { useState } from 'react';

export default function RecipeForm({ initial = {}, onSave, submitLabel = 'Save' }) {
  const [title, setTitle] = useState(initial.title || '');
  const [description, setDescription] = useState(initial.description || '');
  const [ingredientsText, setIngredientsText] = useState((initial.ingredients || []).join('\n'));
  const [instructions, setInstructions] = useState(initial.instructions || '');

  function handleSubmit(e) {
    e.preventDefault();
    const ingredients = ingredientsText
      .split('\n')
      .map(s => s.trim())
      .filter(Boolean);
    onSave({
      title: title.trim(),
      description: description.trim(),
      ingredients,
      instructions: instructions.trim(),
    });
  }

  return (
    <form onSubmit={handleSubmit} className="recipe-form">
      <label>Title</label>
      <input value={title} onChange={e => setTitle(e.target.value)} required />

      <label>Description</label>
      <input value={description} onChange={e => setDescription(e.target.value)} />

      <label>Ingredients (one per line)</label>
      <textarea value={ingredientsText} onChange={e => setIngredientsText(e.target.value)} rows={5} />

      <label>Instructions</label>
      <textarea value={instructions} onChange={e => setInstructions(e.target.value)} rows={8} />

      <div className="form-actions">
        <button type="submit" className="btn primary">{submitLabel}</button>
      </div>
    </form>
  );
}