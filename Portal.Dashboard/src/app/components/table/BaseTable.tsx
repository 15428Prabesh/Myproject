'use client'

import mockData from "@/app/moke/Data"
import { TableContainer,Box,Table,Thead,Tr,Th,Tbody,TableCaption,Td,Tfoot, Button } from "@chakra-ui/react"
import {
    FiXCircle,
    FiCheckCircle,
    FiEdit,
    FiTrash2,
    FiMoreVertical
  } from 'react-icons/fi'
import { Icon } from "@chakra-ui/react"

export default function BaseTable({props,style}:{props?:any,style?:any}){

    const Data:any=mockData
    

    return(
        <Box  bg='gray.700' style={style}>
        <TableContainer>
            <Table variant='unstyled' color='whiteAlpha.800' size='md'>
                <Thead>
                <Tr borderBottom='1px solid' borderBottomColor='whiteAlpha.400'>
                    <Th color='white' fontSize={15}  fontFamily={"initial"}>Title</Th>
                    <Th color='white' fontSize={15}  fontFamily={"initial"}>Summary</Th>
                    <Th color='white' fontSize={15}  fontFamily={"initial"}>Details</Th>
                    <Th color='white' fontSize={15}  fontFamily={"initial"}>Status</Th>
                    <Th color='white' fontSize={15}  fontFamily={"initial"}>Action</Th>
                </Tr>
                </Thead>
                <Tbody>
                {mockData.map((rowData, rowIndex) => (
              <Tr key={rowIndex}>
                <Td>{rowData.Title}</Td>
                <Td>{rowData.Summary}</Td>
                <Td>{rowData.Details}</Td>
                <Td>{
                    rowData.Active?<Icon color={'green.400'}  as={FiCheckCircle}/>:
                    <Icon color={'red.500'} as={FiXCircle}/>}</Td>
                <Td>
                    <Button variant='solid' _hover={{bg:'darkBackground.400'}}   bg='darkBackground.600'  borderRadius='20%' size='sm'>
                        <Icon color={'white'} as={FiMoreVertical}/>
                    </Button>
                </Td>
              </Tr>
                ))}
                </Tbody>
            </Table>
        </TableContainer>
        </Box>
    )
}
