import { useEffect, useState } from 'react';
import RecipeList from '../components/RecipeList';
import { loadAll, removeRecipe } from '../api/recipes';

export default function Home() {
  const [recipes, setRecipes] = useState([]);

  useEffect(() => {
    setRecipes(loadAll());
  }, []);

  function handleDelete(id) {
    if (!confirm('Delete this recipe?')) return;
    removeRecipe(id);
    setRecipes(loadAll());
  }

  return (
    <div>
      <h2>Recipes</h2>
      <RecipeList recipes={recipes} onDelete={handleDelete} />
    </div>
  );
}