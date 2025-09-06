import { useNavigate } from 'react-router-dom';
import RecipeForm from '../components/RecipeForm';
import { addRecipe } from '../api/recipes';

export default function AddRecipe() {
  const nav = useNavigate();

  async function handleSave(data) {
    const created = await addRecipe(data);
    nav(`/view/${created.id}`);
  }

  return (
    <div>
      <h2>Add Recipe</h2>
      <RecipeForm onSave={handleSave} submitLabel="Add Recipe" />
    </div>
  );
}