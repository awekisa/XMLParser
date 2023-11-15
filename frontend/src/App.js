import './App.css';
import { useState } from 'react';
import { ChakraProvider } from '@chakra-ui/react';
import {
	Container,
	Heading,
	Alert,
	AlertIcon,
	Box,
	Spacer,
	VStack,
} from '@chakra-ui/react';
import Form from './Components/Form';

function App() {
	const [responseMessage, setResponseMessage] = useState({});
	const [error, setError] = useState('');

	return (
		<ChakraProvider>
			<Container>
				<Heading>XML Parser</Heading>
				<VStack mt='30px'>
					<Box>
						<Heading size='md'>Please select valid XML file:</Heading>
						<Form
							setResponseMessage={setResponseMessage}
							setError={setError}
						/>
					</Box>
					<Spacer />
					<Box>
						{responseMessage.message ? (
							<Alert
								borderRadius='10px'
								status='success'
							>
								<AlertIcon />
								{responseMessage.message}
							</Alert>
						) : (
							responseMessage.errorMessage && (
								<Alert
									borderRadius='10px'
									status='error'
								>
									<AlertIcon />
									{responseMessage.errorMessage}
								</Alert>
							)
						)}
					</Box>
					{error && (
						<Alert
							borderRadius='10px'
							status='error'
						>
							<AlertIcon />
							{error}
						</Alert>
					)}
				</VStack>
			</Container>
		</ChakraProvider>
	);
}

export default App;
