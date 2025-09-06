import { useParams, Link } from 'react-router-dom';
import { findById } from '../api/recipes';

export default function ViewRecipe() {
  const { id } = useParams();
  const recipe = findById(id);

  if (!recipe) return <p>Recipe not found.</p>;

  return (
    <div>
      <h2>{recipe.title}</h2>
      <p className="muted">{recipe.description}</p>

      <h3>Ingredients</h3>
      <ul>
        {(recipe.ingredients || []).map((ing, i) => <li key={i}>{ing}</li>)}
      </ul> 

      <h3>Instructions</h3>
      <pre className="instructions">{recipe.instructions}</pre>

      <p className="muted">Created: {new Date(recipe.createdDate).toLocaleString()}</p>

      <div className="card-actions">
        <Link to={`/edit/${recipe.id}`} className="btn">Edit</Link>
        <Link to="/" className="btn secondary">Back</Link>
      </div>
    </div>
  );
}