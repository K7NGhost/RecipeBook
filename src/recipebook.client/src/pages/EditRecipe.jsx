import { useNavigate, useParams } from 'react-router-dom';
import RecipeForm from '../components/RecipeForm';
import { findById, updateRecipe } from '../api/recipes';
import { useState } from 'react';

export default function EditRecipe() {
  const { id } = useParams();
  const nav = useNavigate();
  const existing = findById(id) || {};
  const [notFound] = useState(!existing.id);

  if (notFound) {
    return <p>Recipe not found.</p>;
  }

  function handleSave(patch) {
    updateRecipe(id, patch);
    nav(`/view/${id}`);
  }

  return (
    <div>
      <h2>Edit Recipe</h2>
      <RecipeForm initial={existing} onSave={handleSave} submitLabel="Save Changes" />
    </div>
  );
}   