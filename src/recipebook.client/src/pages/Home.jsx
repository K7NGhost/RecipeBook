import { useEffect, useState } from 'react';
import RecipeList from '../components/RecipeList';
import { loadAll, removeRecipe } from '../api/recipes';

export default function Home() {
  const [recipes, setRecipes] = useState([]);

  useEffect(() => {
      let mounted = true;
      (async () => {
          const data = await loadAll();
          if (mounted) setRecipes(data);
      })();
      return () => { mounted = false; };
  }, []);

  async function handleDelete(id) {
      if (!confirm('Delete this recipe?')) return;
      await removeRecipe(id);
      const data = await loadAll();
      setRecipes(data);
  }

  return (
    <div>
      <h2>Recipes</h2>
      <RecipeList recipes={recipes} onDelete={handleDelete} />
    </div>
  );
}