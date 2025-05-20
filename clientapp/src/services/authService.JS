import axios from 'axios';

const API_URL = 'http://localhost:5000/api/auth'; // ajusta tu URL

const login = async (email, password) => {
    const response = await axios.post(`${API_URL}/login`, { email, password });
    const { token, role } = response.data;

    return { token, role }; 
};

export default { login };
